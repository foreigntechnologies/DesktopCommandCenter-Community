using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class TradutorViewModel : ObservableObject
{
    private CancellationTokenSource? _debounceCts;
    private bool _isTranslating;
    private readonly Dictionary<string, string> _langCodes = new()
    {
        { "Detectar Idioma (Auto)", "auto" },
        { "Português", "pt" },
        { "Inglês", "en" },
        { "Espanhol", "es" },
        { "Francês", "fr" },
        { "Alemão", "de" },
        { "Italiano", "it" },
        { "Japonês", "ja" },
        { "Chinês", "zh" },
        { "Russo", "ru" },
        { "Árabe", "ar" },
        { "Coreano", "ko" },
        { "Holandês", "nl" },
        { "Polonês", "pl" },
        { "Turco", "tr" },
        { "Sueco", "sv" },
        { "Norueguês", "no" },
        { "Dinamarquês", "da" },
        { "Finlandês", "fi" },
        { "Grego", "el" },
        { "Hebraico", "he" },
        { "Hindi", "hi" },
        { "Indonésio", "id" },
        { "Vietnamita", "vi" },
        { "Tcheco", "cs" },
        { "Romeno", "ro" },
        { "Húngaro", "hu" },
        { "Ucraniano", "uk" }
    };

    public ObservableCollection<string> SourceLanguages { get; } = new();
    public ObservableCollection<string> TargetLanguages { get; } = new();

    [ObservableProperty]
    private string _sourceText = "";

    [ObservableProperty]
    private string _translatedText = "";

    [ObservableProperty]
    private string _sourceLanguage = "Detectar Idioma (Auto)";

    [ObservableProperty]
    private string _targetLanguage = "Português";

    public TradutorViewModel()
    {
        // Populate lists
        foreach (var lang in _langCodes.Keys)
        {
            SourceLanguages.Add(lang);
            if (lang != "Detectar Idioma (Auto)")
            {
                TargetLanguages.Add(lang);
            }
        }
    }

    partial void OnSourceTextChanged(string value) => DebounceTranslate(true);
    partial void OnTranslatedTextChanged(string value) => DebounceTranslate(false);
    partial void OnSourceLanguageChanged(string value) => DebounceTranslate(true);
    partial void OnTargetLanguageChanged(string value) => DebounceTranslate(true);

    private async void DebounceTranslate(bool forward)
    {
        if (_isTranslating) return;

        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        try
        {
            await Task.Delay(500, token); // Debounce de 500ms
            if (!token.IsCancellationRequested)
            {
                await TranslateInternalAsync(forward);
            }
        }
        catch (TaskCanceledException) { }
    }

    [RelayCommand]
    private async Task TranslateAsync()
    {
        await TranslateInternalAsync(true);
    }

    private async Task TranslateInternalAsync(bool forward)
    {
        string input = forward ? SourceText : TranslatedText;

        if (string.IsNullOrWhiteSpace(input))
        {
            _isTranslating = true;
            if (forward) TranslatedText = string.Empty;
            else SourceText = string.Empty;
            _isTranslating = false;
            return;
        }

        try
        {
            _isTranslating = true;

            if (forward) TranslatedText = "Traduzindo...";
            else SourceText = "Traduzindo...";

            string sl = forward 
                ? (_langCodes.TryGetValue(SourceLanguage, out string? slCode1) ? slCode1 : "auto")
                : (_langCodes.TryGetValue(TargetLanguage, out string? slCode2) ? slCode2 : "pt");

            string tl = forward 
                ? (_langCodes.TryGetValue(TargetLanguage, out string? tlCode1) ? tlCode1 : "pt")
                : (_langCodes.TryGetValue(SourceLanguage, out string? tlCode2) ? tlCode2 : "auto");

            // Se for tradução reversa e a fonte original era Auto, vamos traduzir para inglês como fallback ou pt
            if (tl == "auto") tl = "en";

            using var httpClient = new HttpClient();
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sl}&tl={tl}&dt=t&q={Uri.EscapeDataString(input)}";
            
            var response = await httpClient.GetStringAsync(url);
            
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var segments = root[0];
                if (segments.ValueKind == JsonValueKind.Array)
                {
                    var sb = new StringBuilder();
                    foreach (var segment in segments.EnumerateArray())
                    {
                        if (segment.ValueKind == JsonValueKind.Array && segment.GetArrayLength() > 0)
                        {
                            var textVal = segment[0].GetString();
                            if (!string.IsNullOrEmpty(textVal))
                            {
                                sb.Append(textVal);
                            }
                        }
                    }
                    
                    if (forward) TranslatedText = sb.ToString();
                    else SourceText = sb.ToString();
                    
                    _isTranslating = false;
                    return;
                }
            }
            if (forward) TranslatedText = "Erro: Resposta inesperada do tradutor.";
            else SourceText = "Erro: Resposta inesperada do tradutor.";
        }
        catch (Exception ex)
        {
            if (forward) TranslatedText = $"Erro ao traduzir: {ex.Message}";
            else SourceText = $"Erro ao traduzir: {ex.Message}";
        }
        finally
        {
            _isTranslating = false;
        }
    }

    [RelayCommand]
    private void SwapLanguages()
    {
        var tempSource = SourceLanguage;
        var tempTarget = TargetLanguage;

        if (tempSource == "Detectar Idioma (Auto)")
        {
            SourceLanguage = tempTarget;
            TargetLanguage = "Inglês";
        }
        else
        {
            SourceLanguage = tempTarget;
            TargetLanguage = tempSource;
        }

        var tempText = SourceText;
        SourceText = TranslatedText;
        TranslatedText = tempText;
    }

    [RelayCommand]
    private void CopySource()
    {
        if (!string.IsNullOrEmpty(SourceText))
        {
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(SourceText);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }
    }

    [RelayCommand]
    private void CopyTranslated()
    {
        if (!string.IsNullOrEmpty(TranslatedText))
        {
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(TranslatedText);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }
    }

    [RelayCommand]
    private async Task PasteSourceAsync()
    {
        var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
        if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
        {
            SourceText = await dataPackageView.GetTextAsync();
        }
    }

    [RelayCommand]
    private async Task PasteTranslatedAsync()
    {
        var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
        if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
        {
            TranslatedText = await dataPackageView.GetTextAsync();
        }
    }
}
