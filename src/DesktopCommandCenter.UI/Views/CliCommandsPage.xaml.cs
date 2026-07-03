using Microsoft.UI.Xaml.Controls;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class CliCommandsPage : Page
{
    public CliCommandsViewModel ViewModel { get; }

    public CliCommandsPage()
    {
InitializeComponent();
            UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        ViewModel = new CliCommandsViewModel();
        
    }

        private void UpdateTranslations()
        {
            CliPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Cli_PageTitle");
            CliInputSearchPlaceholderElement.PlaceholderText = Helpers.LocalizationHelper.Instance.GetString("Cli_InputSearch_Placeholder");
// Removed
        }
}



