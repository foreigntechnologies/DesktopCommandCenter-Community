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

    // Maps localized display names → ISO language codes
    private readonly Dictionary<string, string> _langCodes;

    // ISO code → localized display name (for reverse lookup on swap)
    private readonly Dictionary<string, string> _codesToNames;

    private static string L(string key, string fallback) =>
        Helpers.LocalizationHelper.Instance.GetString(key) ?? fallback;

    public TradutorViewModel()
    {
        // Build the localized language map at runtime
        _langCodes = new Dictionary<string, string>
        {
            { L("Lang_Auto",       "Detect Language (Auto)"), "auto" },
            { L("Lang_Portuguese", "Portuguese"),             "pt"   },
            { L("Lang_English",    "English"),                "en"   },
            { L("Lang_Spanish",    "Spanish"),                "es"   },
            { L("Lang_French",     "French"),                 "fr"   },
            { L("Lang_German",     "German"),                 "de"   },
            { L("Lang_Italian",    "Italian"),                "it"   },
            { L("Lang_Japanese",   "Japanese"),               "ja"   },
            { L("Lang_Chinese",    "Chinese"),                "zh"   },
            { L("Lang_Russian",    "Russian"),                "ru"   },
            { L("Lang_Arabic",     "Arabic"),                 "ar"   },
            { L("Lang_Korean",     "Korean"),                 "ko"   },
            { L("Lang_Dutch",      "Dutch"),                  "nl"   },
            { L("Lang_Polish",     "Polish"),                 "pl"   },
            { L("Lang_Turkish",    "Turkish"),                "tr"   },
            { L("Lang_Swedish",    "Swedish"),                "sv"   },
            { L("Lang_Norwegian",  "Norwegian"),              "no"   },
            { L("Lang_Danish",     "Danish"),                 "da"   },
            { L("Lang_Finnish",    "Finnish"),                "fi"   },
            { L("Lang_Greek",      "Greek"),                  "el"   },
            { L("Lang_Hebrew",     "Hebrew"),                 "he"   },
            { L("Lang_Hindi",      "Hindi"),                  "hi"   },
            { L("Lang_Indonesian", "Indonesian"),             "id"   },
            { L("Lang_Vietnamese", "Vietnamese"),             "vi"   },
            { L("Lang_Czech",      "Czech"),                  "cs"   },
            { L("Lang_Romanian",   "Romanian"),               "ro"   },
            { L("Lang_Hungarian",  "Hungarian"),              "hu"   },
            { L("Lang_Ukrainian",  "Ukrainian"),              "uk"   },
        };

        // Reverse map (code → display name) for swap
        _codesToNames = new Dictionary<string, string>();
        foreach (var kv in _langCodes)
            _codesToNames[kv.Value] = kv.Key;

        // Populate UI lists
        foreach (var lang in _langCodes.Keys)
        {
            SourceLanguages.Add(lang);
            if (_langCodes[lang] != "auto")
                TargetLanguages.Add(lang);
        }

        // Set defaults using localized names
        _sourceLanguage = L("Lang_Auto",       "Detect Language (Auto)");
        _targetLanguage = L("Lang_Portuguese", "Portuguese");
    }

    public ObservableCollection<string> SourceLanguages { get; } = new();
    public ObservableCollection<string> TargetLanguages { get; } = new();

    [ObservableProperty]
    private string _sourceText = "";

    [ObservableProperty]
    private string _translatedText = "";

    [ObservableProperty]
    private string _sourceLanguage = "";

    [ObservableProperty]
    private string _targetLanguage = "";

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

            if (forward) TranslatedText = L("Translator_Translating", "Translating...");
            else SourceText = L("Translator_Translating", "Translating...");

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
            if (forward) TranslatedText = L("Translator_ErrorUnexpected", "Error: Unexpected translator response.");
            else SourceText = L("Translator_ErrorUnexpected", "Error: Unexpected translator response.");
        }
        catch (Exception ex)
        {
            if (forward) TranslatedText = L("Translator_ErrorPrefix", "Translation error: ") + ex.Message;
            else SourceText = L("Translator_ErrorPrefix", "Translation error: ") + ex.Message;
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

        // If source was "Auto", swap to English fallback
        if (_langCodes.TryGetValue(tempSource, out var srcCode) && srcCode == "auto")
        {
            SourceLanguage = tempTarget;
            TargetLanguage = _codesToNames.TryGetValue("en", out var en) ? en : tempTarget;
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
