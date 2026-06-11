using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AwakePage : Page
{
    public ViewModels.AwakeViewModel ViewModel { get; }

    public AwakePage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.AwakeViewModel>();
        this.InitializeComponent();
    }
}
