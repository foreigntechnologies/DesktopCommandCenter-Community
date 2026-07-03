using System;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface ITranslationService
{
    string this[string key] { get; }
    string Get(string key);
    /// <summary>Loads translations synchronously. Safe to call from any thread including UI thread.</summary>
    void LoadLanguage(string cultureCode);
    Task SetLanguageAsync(string cultureCode);
    event EventHandler LanguageChanged;
    string CurrentCulture { get; }
}
