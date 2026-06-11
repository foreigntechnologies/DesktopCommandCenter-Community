using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class CapturaPage : Page
{
    public ViewModels.CapturaViewModel ViewModel { get; }

    public CapturaPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.CapturaViewModel>();
        this.InitializeComponent();
    }
}
