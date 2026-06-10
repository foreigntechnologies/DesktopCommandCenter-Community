using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ColorPickerPage : Page
{
    public ViewModels.ColorPickerViewModel ViewModel { get; }

    public ColorPickerPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.ColorPickerViewModel>();
    }
}
