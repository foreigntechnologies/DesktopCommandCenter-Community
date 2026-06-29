using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class IALocalPage : Page
{
    public ViewModels.IALocalViewModel ViewModel { get; }

    public IALocalPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.IALocalViewModel>();
        this.InitializeComponent();
        Loaded += IALocalPage_Loaded;
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    private void IALocalPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        UpdateTranslations();
    }

    private void UpdateTranslations()
    {
        // Traduções agora devem ser aplicadas diretamente via x:Bind no XAML
    }
}
