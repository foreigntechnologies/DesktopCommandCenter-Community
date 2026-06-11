using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class TemporizadorPage : Page
{
    public ViewModels.TemporizadorViewModel ViewModel { get; }

    public TemporizadorPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.TemporizadorViewModel>();
        this.InitializeComponent();
    }
}
