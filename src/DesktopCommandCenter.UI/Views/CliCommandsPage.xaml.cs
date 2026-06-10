using Microsoft.UI.Xaml.Controls;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class CliCommandsPage : Page
{
    public CliCommandsViewModel ViewModel { get; }

    public CliCommandsPage()
    {
        ViewModel = new CliCommandsViewModel();
        InitializeComponent();
    }
}
