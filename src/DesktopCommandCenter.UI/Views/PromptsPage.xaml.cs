using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class PromptsPage : Page
{
    public ViewModels.PromptsViewModel ViewModel { get; }

    public PromptsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.PromptsViewModel>();
        this.InitializeComponent();
        this.Loaded += async (s, e) => await ViewModel.LoadPromptsAsync();
    }
}
