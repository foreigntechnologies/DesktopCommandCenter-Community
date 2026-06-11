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
    }
}
