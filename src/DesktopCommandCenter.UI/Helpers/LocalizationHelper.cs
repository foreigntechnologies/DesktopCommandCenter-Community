using DesktopCommandCenter.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.Helpers;

public class LocalizationHelper : INotifyPropertyChanged
{
    private static LocalizationHelper? _instance;
    public static LocalizationHelper Instance => _instance ??= new LocalizationHelper();

    private readonly ITranslationService _translationService;

    /// <summary>Task que completa quando o idioma inicial terminar de carregar.</summary>
    public Task WhenReady { get; }

    private LocalizationHelper()
    {
        _translationService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<ITranslationService>();
        _translationService.LanguageChanged += (s, e) => OnPropertyChanged("Item[]");

        // Carrega sincronamente sem risco de deadlock na thread de UI
        var lang = App.GetAppLanguage();
        _translationService.LoadLanguage(lang);

        // WhenReady já está completo pois a carga é síncrona
        WhenReady = Task.CompletedTask;
    }

    public string CurrentCulture => _translationService.CurrentCulture;

    public string this[string key] => _translationService.Get(key);

    // Método explícito para suportar {x:Bind} com passagem de parâmetro em XAML,
    // pois {x:Bind helpers:LocalizationHelper.Instance['Chave']} causa erro WMC9999.
    public string GetString(string key) => _translationService.Get(key);

    // Método estático para evitar erro WMC9999 ao invocar método de instância via propriedade estática no XAML
    public static string GetLocalized(string key) => Instance.GetString(key);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
