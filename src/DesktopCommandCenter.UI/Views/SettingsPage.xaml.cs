using Microsoft.UI.Xaml.Controls;
using DesktopCommandCenter.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<SettingsViewModel>();
        this.InitializeComponent();
    }
}
