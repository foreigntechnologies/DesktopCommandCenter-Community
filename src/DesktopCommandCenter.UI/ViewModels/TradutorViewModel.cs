using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class TradutorViewModel : ObservableObject
{
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

    [RelayCommand]
    private async Task TranslateAsync()
    {
        if (string.IsNullOrWhiteSpace(SourceText))
        {
            TranslatedText = string.Empty;
            return;
        }

        try
        {
            TranslatedText = "Traduzindo...";

            string sl = _langCodes.TryGetValue(SourceLanguage, out string? slCode) ? slCode : "auto";
            string tl = _langCodes.TryGetValue(TargetLanguage, out string? tlCode) ? tlCode : "pt";

            using var httpClient = new HttpClient();
            // Google Translate free endpoint
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sl}&tl={tl}&dt=t&q={Uri.EscapeDataString(SourceText)}";
            
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
                    TranslatedText = sb.ToString();
                    return;
                }
            }
            TranslatedText = "Erro: Resposta inesperada do tradutor.";
        }
        catch (Exception ex)
        {
            TranslatedText = $"Erro ao traduzir: {ex.Message}";
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
}
