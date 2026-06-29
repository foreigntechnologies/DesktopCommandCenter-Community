using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class ChatMessage : ObservableObject
{
    public string Role { get; set; } = string.Empty;
    
    [ObservableProperty]
    private string _content = string.Empty;
    
    public bool IsUser => !Role.StartsWith("ChatFT");
    public string? ImagePath { get; set; } = null;
    public bool HasImage => !string.IsNullOrEmpty(ImagePath);

    [ObservableProperty]
    private bool _hasDownloadAction = false;

    [ObservableProperty]
    private bool _isThinking = false;
    
    public Microsoft.UI.Xaml.Visibility ThinkingVisibility => IsThinking ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
}

public partial class IALocalViewModel : ObservableObject
{
    private readonly IIAAgentService _agentService;
    private readonly IWhisperTranscriptionService _whisperService;

    [ObservableProperty]
    private string _currentPrompt = "";

    [ObservableProperty]
    private bool _isGenerating = false;

    [ObservableProperty]
    private bool _isRecording = false;

    [ObservableProperty]
    private string _attachedImagePath = "";

    [ObservableProperty]
    private string _statusMessage = "Pronto. Ollama + Semantic Kernel.";

    [ObservableProperty]
    private bool _isDownloadingModel = false;

    [ObservableProperty]
    private double _downloadProgressValue = 0;

    public bool IsNotDownloadingModel => !IsDownloadingModel;

    public ObservableCollection<ChatMessage> Messages { get; } = new();
    
    public ObservableCollection<string> AvailableModels { get; } = new();

    [ObservableProperty]
    private string _selectedModel = "";

    [ObservableProperty]
    private string _userName = "User";

    private readonly IAuthService _authService;

    public IALocalViewModel(IIAAgentService agentService, IWhisperTranscriptionService whisperService, IAuthService authService)
    {
        _agentService = agentService;
        _whisperService = whisperService;
        _authService = authService;
        _agentService.ClearHistory();
        
        var welcomeMsg = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("ChatFT_WelcomeMessage");
        if (string.IsNullOrEmpty(welcomeMsg)) welcomeMsg = "Para utilizar o ChatFT localmente, é necessário ter o Ollama instalado em sua máquina! Ou utilizar uma secret key do Gemini (Google AI Studio), Claude ou OpenAI.";
        Messages.Add(new ChatMessage { Role = "ChatFT (Sistema)", Content = welcomeMsg });

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
        }
        else
        {
            var noModelMsg = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("ChatFT_NoModelsMessage");
            if (string.IsNullOrEmpty(noModelMsg)) 
                noModelMsg = "Nenhum modelo local encontrado. Para usar o ChatFT, você precisa baixar um modelo de Inteligência Artificial. Qual você deseja baixar?";

            Messages.Add(new ChatMessage { Role = "ChatFT (Sistema)", Content = noModelMsg, HasDownloadAction = true });
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

            foreach (var msg in Messages)
            {
                msg.HasDownloadAction = false;
            }

            var successMsg = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("ChatFT_DownloadSuccess");
            if (string.IsNullOrEmpty(successMsg)) successMsg = "Modelo {modelName} baixado com sucesso! Agora você já pode conversar.";
            Messages.Add(new ChatMessage { Role = "ChatFT (Sistema)", Content = successMsg });
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
            StatusMessage = "Transcrevendo arquivo de áudio...";
            IsGenerating = true;
            try
            {
                var text = await _whisperService.TranscribeFileAsync(file.Path);
                if (!string.IsNullOrEmpty(text))
                {
                    CurrentPrompt += " " + text;
                }
                StatusMessage = "Pronto.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro na transcrição: {ex.Message}";
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
            StatusMessage = "Processando áudio...";
            IsRecording = false;
            IsGenerating = true;
            try
            {
                var text = await _whisperService.StopRecordingAndTranscribeAsync();
                if (!string.IsNullOrEmpty(text))
                {
                    CurrentPrompt += " " + text;
                }
                StatusMessage = "Pronto.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro na transcrição: {ex.Message}";
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
                StatusMessage = "Gravando microfone... Clique novamente para parar.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao gravar: {ex.Message}";
            }
        }
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
                await foreach (var token in _agentService.SendMessageStreamAsync(promptBackup, imageBackup).ConfigureAwait(false))
                {
                    dispatcher.TryEnqueue(() => 
                    {
                        aiMsg.IsThinking = false;
                        aiMsg.Content += token;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChatFT Error] {ex}");
                dispatcher.TryEnqueue(() => 
                {
                    aiMsg.Content += $"\n[Erro: {ex.Message}]";
                    StatusMessage = "Erro ao contatar IA.";
                });
            }
            finally
            {
                dispatcher.TryEnqueue(() => 
                {
                    IsGenerating = false;
                    aiMsg.IsThinking = false;
                });
            }
        });
    }

    public string GetString(string key) => DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString(key);
}
