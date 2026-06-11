using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class TradutorPage : Page
{
    public ViewModels.TradutorViewModel ViewModel { get; }

    public TradutorPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.TradutorViewModel>();
        this.InitializeComponent();
    }
}
