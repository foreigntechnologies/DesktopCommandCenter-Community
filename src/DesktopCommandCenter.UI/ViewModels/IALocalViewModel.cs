using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class ChatMessage : ObservableObject
{
    public ChatMessage()
    {
        // Notify HasSources when items are added/removed from the collection
        SearchSources.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasSources));
    }

    public string Role { get; set; } = string.Empty;
    
    [ObservableProperty]
    private string _content = string.Empty;
    
    public bool IsUserBool => !Role.StartsWith("ChatFT");
    public bool IsAIBool => Role.StartsWith("ChatFT");
    
    // Visibility helpers for x:Bind (no converter needed)
    public Microsoft.UI.Xaml.Visibility IsUser => IsUserBool ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
    public Microsoft.UI.Xaml.Visibility IsAI => IsAIBool ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
    
    public string? ImagePath { get; set; } = null;
    public Microsoft.UI.Xaml.Visibility HasImage => !string.IsNullOrEmpty(ImagePath) ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;

    // Web search sources attached to this AI message
    public System.Collections.ObjectModel.ObservableCollection<DesktopCommandCenter.Application.Interfaces.WebSearchResult> SearchSources { get; } = new();
    public Microsoft.UI.Xaml.Visibility HasSources => SearchSources.Count > 0 ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;

    [ObservableProperty]
    private bool _hasDownloadAction = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ThinkingVisibility))]
    private bool _isThinking = false;
    
    public Microsoft.UI.Xaml.Visibility ThinkingVisibility => IsThinking ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
}

public partial class IALocalViewModel : ObservableObject
{
    private readonly IIAAgentService _agentService;
    private readonly IWhisperTranscriptionService _whisperService;
    private readonly IWebSearchService _webSearchService;

    [ObservableProperty]
    private string _currentPrompt = "";

    [ObservableProperty]
    private bool _isGenerating = false;

    [ObservableProperty]
    private bool _isRecording = false;

    [ObservableProperty]
    private string _attachedImagePath = "";

    [ObservableProperty]
    private string _statusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("ChatFT_Ready") ?? "Pronto. Ollama + Semantic Kernel.";

    [ObservableProperty]
    private bool _isDownloadingModel = false;

    [ObservableProperty]
    private double _downloadProgressValue = 0;

    public bool IsNotDownloadingModel => !IsDownloadingModel;

    public ObservableCollection<ChatMessage> Messages { get; } = new();
    
    public ObservableCollection<string> AvailableModels { get; } = new();

    [ObservableProperty]
    private string _selectedModel = "";

    public bool IsChatEmpty => Messages.Count == 0;
    public bool IsChatNotEmpty => Messages.Count > 0;

    [ObservableProperty]
    private bool _isWebSearchMode = false; // Default: OFF (natural chat)

    [ObservableProperty]
    private bool _isEditMenuOpen = false;

    [ObservableProperty]
    private bool _showNoModelsWarning = false;

    [ObservableProperty]
    private string _userName = "User";

    private readonly IAuthService _authService;

    public IALocalViewModel(IIAAgentService agentService, IWhisperTranscriptionService whisperService, IAuthService authService, IWebSearchService webSearchService)
    {
        _agentService = agentService;
        _whisperService = whisperService;
        _authService = authService;
        _webSearchService = webSearchService;
        _agentService.ClearHistory();

        Messages.CollectionChanged += (s, e) => 
        {
            OnPropertyChanged(nameof(IsChatEmpty));
            OnPropertyChanged(nameof(IsChatNotEmpty));
        };

        _ = LoadModelsAsync();
        _ = LoadUserNameAsync();
    }

    partial void OnSelectedModelChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _agentService.SetModel(value);
        }
    }

    private async Task LoadUserNameAsync()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user != null && !string.IsNullOrEmpty(user.DisplayName))
            {
                UserName = user.DisplayName;
            }
        }
        catch { }
    }

    private async Task LoadModelsAsync()
    {
        var models = await _agentService.GetAvailableModelsAsync();
        AvailableModels.Clear();
        foreach (var m in models)
        {
            AvailableModels.Add(m);
        }

        if (AvailableModels.Count > 0)
        {
            if (AvailableModels.Contains("phi3"))
            {
                SelectedModel = "phi3";
            }
            else
            {
                SelectedModel = AvailableModels[0];
            }
            ShowNoModelsWarning = false;
        }
        else
        {
            ShowNoModelsWarning = true;
        }
    }

    [RelayCommand]
    private async Task DownloadModelAsync(string modelName)
    {
        if (IsDownloadingModel || string.IsNullOrWhiteSpace(modelName)) return;

        IsDownloadingModel = true;
        DownloadProgressValue = 0;
        OnPropertyChanged(nameof(IsNotDownloadingModel));

        try
        {
            var progress = new Progress<double>(percent =>
            {
                DownloadProgressValue = percent;
            });

            await _agentService.PullModelAsync(modelName, progress);
            
            await LoadModelsAsync();
            if (AvailableModels.Contains(modelName))
            {
                SelectedModel = modelName;
            }

            ShowNoModelsWarning = false;
        }
        catch (Exception ex)
        {
            var errorMsg = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("ChatFT_DownloadError");
            if (string.IsNullOrEmpty(errorMsg)) errorMsg = "Erro ao baixar modelo.";
            Messages.Add(new ChatMessage { Role = "ChatFT (Sistema)", Content = $"{errorMsg} {ex.Message}" });
        }
        finally
        {
            IsDownloadingModel = false;
            DownloadProgressValue = 0;
            OnPropertyChanged(nameof(IsNotDownloadingModel));
        }
    }

    [RelayCommand]
    private async Task AttachImageAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow));
        picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            AttachedImagePath = file.Path;
        }
    }

    [RelayCommand]
    private async Task AttachAudioAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow));
        picker.FileTypeFilter.Add(".mp3");
        picker.FileTypeFilter.Add(".wav");

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            StatusMessage = GetString("ChatFT_StatusTranscribing", "Transcrevendo arquivo de áudio...");
            IsGenerating = true;
            try
            {
                var text = await _whisperService.TranscribeFileAsync(file.Path);
                if (!string.IsNullOrEmpty(text))
                {
                    CurrentPrompt += " " + text;
                }
                StatusMessage = GetString("ChatFT_StatusReady", "Pronto.");
            }
            catch (Exception ex)
            {
                StatusMessage = GetString("ChatFT_StatusErrorTranscribing", "Erro na transcrição: ") + ex.Message;
            }
            finally
            {
                IsGenerating = false;
            }
        }
    }

    [RelayCommand]
    private async Task ToggleRecordingAsync()
    {
        if (IsRecording)
        {
            StatusMessage = GetString("ChatFT_StatusProcessingAudio", "Processando áudio...");
            IsRecording = false;
            IsGenerating = true;
            try
            {
                var text = await _whisperService.StopRecordingAndTranscribeAsync();
                if (!string.IsNullOrEmpty(text))
                {
                    CurrentPrompt += " " + text;
                }
                StatusMessage = GetString("ChatFT_StatusReady", "Pronto.");
            }
            catch (Exception ex)
            {
                StatusMessage = GetString("ChatFT_StatusErrorTranscribing", "Erro na transcrição: ") + ex.Message;
            }
            finally
            {
                IsGenerating = false;
            }
        }
        else
        {
            try
            {
                _whisperService.StartRecording();
                IsRecording = true;
                StatusMessage = GetString("ChatFT_StatusRecording", "Gravando microfone... Clique novamente para parar.");
            }
            catch (Exception ex)
            {
                StatusMessage = GetString("ChatFT_StatusErrorRecording", "Erro ao gravar: ") + ex.Message;
            }
        }
    }

    [RelayCommand]
    private void ToggleWebSearchMode()
    {
        IsWebSearchMode = !IsWebSearchMode;
    }

    [RelayCommand]
    private void SetPrompt(string prompt)
    {
        CurrentPrompt = prompt;
    }

    [RelayCommand]
    private void ClearChat()
    {
        Messages.Clear();
        _agentService.ClearHistory();
        CurrentPrompt = "";
        AttachedImagePath = "";
        StatusMessage = GetString("ChatFT_StatusReady", "Pronto.");
    }

    [RelayCommand]
    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(CurrentPrompt) && string.IsNullOrEmpty(AttachedImagePath)) return;

        var userMsg = new ChatMessage 
        { 
            Role = _userName, 
            Content = CurrentPrompt,
            ImagePath = AttachedImagePath
        };
        Messages.Add(userMsg);
        
        bool isWebSearch = IsWebSearchMode;
        // Keep web search mode ON after sending (persistent, like ChatGPT)
        
        var promptBackup = CurrentPrompt;
        var imageBackup = AttachedImagePath;

        CurrentPrompt = "";
        AttachedImagePath = "";
        IsGenerating = true;

        var aiMsg = new ChatMessage { Role = "ChatFT", Content = "", IsThinking = true };
        Messages.Add(aiMsg);

        var dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        _ = Task.Run(async () =>
        {
            try
            {
                string effectivePrompt = promptBackup;

                // Evitar busca na web para cumprimentos simples ou testes curtos
                bool isGreeting = System.Text.RegularExpressions.Regex.IsMatch(promptBackup.Trim(), @"^(\s*(oi|oie|ol[aá]|bom dia|boa tarde|boa noite|hello|hi|hey|e a[ií]|tudo bem|tudo bom|como vai|como est[aá]|fala a[ií]|teste|testando)[,\.\!\?]*\s*)+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (isGreeting)
                {
                    isWebSearch = false;
                }

                // If web search mode, search the web first and inject results
                if (isWebSearch)
                {
                    dispatcher.TryEnqueue(() =>
                    {
                        StatusMessage = GetString("ChatFT_StatusSearching", "Buscando na web...");
                        aiMsg.Content = "⏳ " + GetString("ChatFT_StatusSearching", "Buscando na web...");
                        aiMsg.IsThinking = false;
                    });

                    var searchResults = await _webSearchService.SearchAsync(promptBackup, maxResults: 5);

                    if (searchResults.Count > 0)
                    {
                        var now = DateTime.Now;
                        var ctx = new StringBuilder();
                        ctx.AppendLine($"Data e hora atual: {now:dddd, dd 'de' MMMM 'de' yyyy, HH:mm} (horário de Brasília)");
                        ctx.AppendLine();
                        ctx.AppendLine("INSTRUÇÃO CRÍTICA: Você está atuando como assistente com acesso à web. Os dados abaixo foram obtidos de uma busca na internet realizada AGORA, em tempo real. Estes dados são mais recentes e confiáveis do que qualquer coisa no seu treinamento.");
                        ctx.AppendLine("REGRA: Baseie sua resposta UNICAMENTE nas fontes abaixo. Se as fontes afirmam uma data específica, cite essa data exatamente. NÃO diga 'ainda em desenvolvimento' se as fontes indicam uma data confirmada.");
                        ctx.AppendLine();
                        foreach (var (r, i) in searchResults.Select((r, i) => (r, i + 1)))
                        {
                            ctx.AppendLine($"--- FONTE {i}: {r.Title} ---");
                            ctx.AppendLine(r.Snippet);
                            if (!string.IsNullOrEmpty(r.Url)) ctx.AppendLine($"Origem: {r.Url}");
                            ctx.AppendLine();
                        }
                        ctx.AppendLine($"PERGUNTA DO USUÁRIO: {promptBackup}");
                        ctx.AppendLine();
                        
                        var appLang = DesktopCommandCenter.UI.App.GetAppLanguage();
                        string targetLang = appLang.StartsWith("en") ? "English" : appLang.StartsWith("es") ? "Español" : "português do Brasil";
                        ctx.AppendLine($"Responda de forma direta, completa e em {targetLang}. Inclua datas, nomes e detalhes específicos que aparecem nas fontes. Seja preciso como o ChatGPT.");
                        effectivePrompt = ctx.ToString();

                        // Store sources on the message for display
                        dispatcher.TryEnqueue(() =>
                        {
                            var searchStatus = GetString("ChatFT_StatusFoundResults", "Encontrei {0} resultado(s). Analisando...");
                            StatusMessage = searchStatus.Contains("{0}") ? string.Format(searchStatus, searchResults.Count) : searchStatus;
                            aiMsg.Content = "";
                            aiMsg.IsThinking = true;
                            foreach (var r in searchResults)
                                aiMsg.SearchSources.Add(r);
                        });
                    }
                    else
                    {
                        dispatcher.TryEnqueue(() =>
                        {
                            StatusMessage = GetString("ChatFT_StatusNoResults", "Nenhum resultado encontrado. Respondendo com conhecimento local...");
                            aiMsg.Content = "";
                            aiMsg.IsThinking = true;
                        });
                    }
                }

                string buffer = "";
                var lastUpdate = DateTime.UtcNow;

                await foreach (var token in _agentService.SendMessageStreamAsync(effectivePrompt, imageBackup).ConfigureAwait(false))
                {
                    buffer += token;
                    var now = DateTime.UtcNow;
                    if ((now - lastUpdate).TotalMilliseconds >= 150)
                    {
                        var chunk = buffer;
                        buffer = "";
                        lastUpdate = now;
                        dispatcher.TryEnqueue(() => 
                        {
                            aiMsg.IsThinking = false;
                            aiMsg.Content += chunk;
                        });
                    }
                }

                if (buffer.Length > 0)
                {
                    dispatcher.TryEnqueue(() => 
                    {
                        aiMsg.IsThinking = false;
                        aiMsg.Content += buffer;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChatFT Error] {ex}");
                dispatcher.TryEnqueue(() => 
                {
                    var errText = GetString("ChatFT_StatusErrorAI", "Erro ao contatar IA.");
                    aiMsg.Content += $"\n[{errText} {ex.Message}]";
                    StatusMessage = errText;
                });
            }
            finally
            {
                dispatcher.TryEnqueue(() => 
                {
                    IsGenerating = false;
                    aiMsg.IsThinking = false;
                    StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("ChatFT_Ready") ?? "Pronto. Ollama + Semantic Kernel.";
                });
            }
        });
    }

    public string GetString(string key, string fallback = "") 
    {
        var result = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString(key);
        return string.IsNullOrEmpty(result) || result == key ? fallback : result;
    }
}
