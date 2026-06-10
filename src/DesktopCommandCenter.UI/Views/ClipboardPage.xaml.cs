using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ClipboardPage : Page
{
    public ViewModels.ClipboardViewModel ViewModel { get; }

    public ClipboardPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.ClipboardViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _ = ViewModel.LoadItemsAsync();
    }
}
