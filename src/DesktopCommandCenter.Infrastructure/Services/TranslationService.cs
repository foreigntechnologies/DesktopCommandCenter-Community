using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Services;

public class TranslationService : ITranslationService
{
    private Dictionary<string, string> _translations = new();
    private readonly string _resourcesPath;

    public string CurrentCulture { get; private set; } = "en-US";

    public event EventHandler? LanguageChanged;

    public TranslationService()
    {
        _resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        Directory.CreateDirectory(_resourcesPath);
    }

    public string this[string key] => Get(key);

    public string Get(string key)
    {
        return _translations.TryGetValue(key, out var value) ? value : key;
    }

    /// <summary>Loads translations synchronously using blocking I/O — safe on any thread.</summary>
    public void LoadLanguage(string cultureCode)
    {
        var filePath = Path.Combine(_resourcesPath, $"{cultureCode}.json");

        if (!File.Exists(filePath))
            filePath = Path.Combine(_resourcesPath, "en-US.json"); // fallback

        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        else
        {
            _translations.Clear();
        }

        CurrentCulture = cultureCode;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task SetLanguageAsync(string cultureCode)
    {
        var filePath = Path.Combine(_resourcesPath, $"{cultureCode}.json");
        
        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            CurrentCulture = cultureCode;
        }
        else
        {
            // Fallback to en-US if requested culture not found
            var fallbackPath = Path.Combine(_resourcesPath, "en-US.json");
            if (File.Exists(fallbackPath))
            {
                var json = await File.ReadAllTextAsync(fallbackPath);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
            else
            {
                _translations.Clear();
            }
            CurrentCulture = cultureCode;
        }

        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
