using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AutomacoesPage : Page
{
    public ViewModels.AutomacoesViewModel ViewModel { get; }

    public AutomacoesPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.AutomacoesViewModel>();
        this.InitializeComponent();
    }
}
