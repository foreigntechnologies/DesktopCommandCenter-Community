using System;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface ITranslationService
{
    string this[string key] { get; }
    string Get(string key);
    Task SetLanguageAsync(string cultureCode);
    event EventHandler LanguageChanged;
    string CurrentCulture { get; }
}
