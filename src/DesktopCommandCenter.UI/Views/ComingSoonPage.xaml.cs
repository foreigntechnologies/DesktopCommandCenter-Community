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
            SubtitleText.Text = $"Estamos construindo o mÃ³dulo '{featureName}' para vocÃª. Fique de olho nas prÃ³ximas atualizaÃ§Ãµes!";
        }
    }

        private void UpdateTranslations()
        {
            ComingSoonTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("ComingSoon_Title");
            SubtitleText.Text = Helpers.LocalizationHelper.Instance.GetString("ComingSoon_Subtitle");
        }
}


