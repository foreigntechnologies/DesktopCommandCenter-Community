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

    public async Task SetLanguageAsync(string cultureCode)
    {
        var filePath = Path.Combine(_resourcesPath, $"{cultureCode}.json");
        
        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            CurrentCulture = cultureCode;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            // Fallback empty if not found, to avoid crash.
            _translations.Clear();
            CurrentCulture = cultureCode;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
