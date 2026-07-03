using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class OobeDialog : ContentDialog
{
    public OobeDialog()
    {

        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        this.PrimaryButtonClick += OobeDialog_PrimaryButtonClick;
    }

    private void OobeDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        string themeStr = ThemeComboBox.SelectedIndex switch
        {
            0 => "Light",
            1 => "Dark",
            _ => "Default"
        };
        
        App.SaveTheme(themeStr);
        App.ApplyTheme(themeStr);
    }

        private void UpdateTranslations()
        {
            OobeTitleElement.Title = Helpers.LocalizationHelper.Instance.GetString("Oobe_Title");
            OobeDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("Oobe_Desc");
            OobeThemeLightElement.Content = Helpers.LocalizationHelper.Instance.GetString("Oobe_ThemeLight");
            OobeThemeDarkElement.Content = Helpers.LocalizationHelper.Instance.GetString("Oobe_ThemeDark");
            OobeThemeSystemElement.Content = Helpers.LocalizationHelper.Instance.GetString("Oobe_ThemeSystem");
            OobeHintElement.Text = Helpers.LocalizationHelper.Instance.GetString("Oobe_Hint");
        }
}


