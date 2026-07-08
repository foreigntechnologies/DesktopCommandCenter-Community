using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ComingSoonPage : Page
{
    public ComingSoonPage()
    {

        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        if (e.Parameter is string featureName)
        {
            SubtitleText.Text = $"Estamos construindo o módulo '{featureName}' para você. Fique de olho nas próximas atualizações!";
        }
    }

        private void UpdateTranslations()
        {
            ComingSoonTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("ComingSoon_Title");
            SubtitleText.Text = Helpers.LocalizationHelper.Instance.GetString("ComingSoon_Subtitle");
        }
}


