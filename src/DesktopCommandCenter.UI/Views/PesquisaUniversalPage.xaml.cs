using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class PesquisaUniversalPage : Page
{
    public ViewModels.PesquisaUniversalViewModel ViewModel { get; }

    public PesquisaUniversalPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.PesquisaUniversalViewModel>();
        this.InitializeComponent();
    }
}
