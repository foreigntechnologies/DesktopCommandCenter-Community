using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ComingSoonPage : Page
{
    public ComingSoonPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        if (e.Parameter is string featureName)
        {
            SubtitleText.Text = $"Estamos construindo o módulo '{featureName}' para você. Fique de olho nas próximas atualizações!";
        }
    }
}
