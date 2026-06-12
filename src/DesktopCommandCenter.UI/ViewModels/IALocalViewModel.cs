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
    public string Content { get; set; } = string.Empty;
    public bool IsUser => Role == "User";
    public string ImagePath { get; set; } = string.Empty;
    public bool HasImage => !string.IsNullOrEmpty(ImagePath);
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

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    public IALocalViewModel(IIAAgentService agentService, IWhisperTranscriptionService whisperService)
    {
        _agentService = agentService;
        _whisperService = whisperService;
        _agentService.ClearHistory();
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
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentPrompt) && string.IsNullOrEmpty(AttachedImagePath)) return;

        var userMsg = new ChatMessage 
        { 
            Role = "User", 
            Content = CurrentPrompt,
            ImagePath = AttachedImagePath
        };
        Messages.Add(userMsg);
        
        var promptBackup = CurrentPrompt;
        var imageBackup = AttachedImagePath;

        CurrentPrompt = "";
        AttachedImagePath = "";
        IsGenerating = true;
        StatusMessage = "A IA está pensando (Acessando ferramentas...)...";

        var aiMsg = new ChatMessage { Role = "ChatFT", Content = "" };
        Messages.Add(aiMsg);

        try
        {
            await foreach (var token in _agentService.SendMessageStreamAsync(promptBackup, imageBackup))
            {
                aiMsg.Content += token;
            }
            StatusMessage = "Pronto.";
        }
        catch (Exception ex)
        {
            aiMsg.Content += $"\n[Erro: {ex.Message}]";
            StatusMessage = "Erro ao contatar IA.";
        }
        finally
        {
            IsGenerating = false;
        }
    }
}
