using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AlwaysOnTopPage : Page
{
    public ViewModels.AlwaysOnTopViewModel ViewModel { get; }

    public AlwaysOnTopPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.AlwaysOnTopViewModel>();
        this.InitializeComponent();
    }
}
