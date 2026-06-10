using DesktopCommandCenter.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace DesktopCommandCenter.UI.Helpers;

public class LocalizationHelper : INotifyPropertyChanged
{
    private static LocalizationHelper? _instance;
    public static LocalizationHelper Instance => _instance ??= new LocalizationHelper();

    private readonly ITranslationService _translationService;

    private LocalizationHelper()
    {
        _translationService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<ITranslationService>();
        _translationService.LanguageChanged += (s, e) => OnPropertyChanged("Item[]");
        
        // Carregar idioma padrão no início
        _ = _translationService.SetLanguageAsync("pt-BR");
    }

    public string this[string key] => _translationService.Get(key);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
