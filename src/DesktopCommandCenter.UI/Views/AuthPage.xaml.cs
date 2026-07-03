using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AuthPage : Page
{
    public AuthViewModel ViewModel { get; }

    public AuthPage()
    {
InitializeComponent();
            UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<AuthViewModel>();
        
        Loaded += AuthPage_Loaded;
        // Translate.Key on each XAML element handles live language updates automatically.
    }

    private void AuthPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Translations are handled automatically by helpers:Translate.Key on each XAML element.
        // No manual UpdateTranslations() needed here.
    }

    public Microsoft.UI.Xaml.Media.ImageSource? ConvertToImageSource(string url)
    {
        if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return null;
        try
        {
            return new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(uri);
        }
        catch
        {
            return null;
        }
    }

    private async void OpenCommunityRepo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/KoglinDev/DesktopCommandCenter-Community"));
    }

        private void UpdateTranslations()
        {
            TxtAuthTitle.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_PageTitle");
            TxtAuthDesc.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_PageDesc");
            TxtLoading.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_TxtLoading");
            if (AuthBtnCancelElement.Content is string || AuthBtnCancelElement.Content == null) AuthBtnCancelElement.Content = Helpers.LocalizationHelper.Instance.GetString("Auth_BtnCancel");
            TxtGoogle.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_BtnGoogle");
            TxtGitHub.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_BtnGitHub");
            // TODO: Implement translation for TxtTermsPrefix of type Run
            // TODO: Implement translation for HypTerms of type Run
            // TODO: Implement translation for TxtTermsMid of type Run
            // TODO: Implement translation for HypPrivacy of type Run
            // TODO: Implement translation for TxtTermsSuffix of type Run
            TxtLinkAccountsTitle.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_LinkAccountsTitle");
            TxtLinkGoogle.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_BtnLinkGoogle");
            TxtLinkGitHub.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_BtnLinkGitHub");
            TxtLinkDesc.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_LinkDesc");
            TxtProActiveTitle.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProActiveTitle");
            TxtProActiveDesc.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProActiveDesc");
            TxtProManage.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProManage");
            TxtPausedActiveTitle.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_PausedTitle");
            TxtPausedActiveDesc.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_PausedDesc");
            TxtPausedManage.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_PausedManage");
            if (BtnLogout.Content is string || BtnLogout.Content == null) BtnLogout.Content = Helpers.LocalizationHelper.Instance.GetString("Auth_BtnLogout");
            SettingsSubTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_SubTitle");
            // TODO: Implement translation for SettingsCommF1BElement of type Run
            // TODO: Implement translation for SettingsCommF1DElement of type Run
            // TODO: Implement translation for SettingsCommF2BElement of type Run
            // TODO: Implement translation for SettingsCommF2DElement of type Run
            // TODO: Implement translation for SettingsCommF3BElement of type Run
            // TODO: Implement translation for SettingsCommF3DElement of type Run
            // TODO: Implement translation for SettingsCommF4BElement of type Run
            // TODO: Implement translation for SettingsCommF4DElement of type Run
            // TODO: Implement translation for SettingsCommF5BElement of type Run
            // TODO: Implement translation for SettingsCommF5DElement of type Run
            // TODO: Implement translation for SettingsCommF6BElement of type Run
            // TODO: Implement translation for SettingsCommF6DElement of type Run
            // TODO: Implement translation for SettingsCommF7BElement of type Run
            // TODO: Implement translation for SettingsCommF7DElement of type Run
            // TODO: Implement translation for SettingsCommF8BElement of type Run
            // TODO: Implement translation for SettingsCommF8DElement of type Run
            // TODO: Implement translation for SettingsCommF9BElement of type Run
            // TODO: Implement translation for SettingsCommF9DElement of type Run
            // TODO: Implement translation for SettingsCommF10BElement of type Run
            // TODO: Implement translation for SettingsCommF10DElement of type Run
            SettingsCommF11Element.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_CommF11");
            SettingsCommF13Element.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_CommF13");
            SettingsCommF14Element.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_CommF14");
            SettingsProBadgeElement.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_ProBadge");
            SettingsProSubtitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_ProSubtitle");
            SettingsProDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_ProDesc");
            SettingsProF1Element.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_ProF1");
            // TODO: Implement translation for SettingsProF2BElement of type Run
            // TODO: Implement translation for SettingsProF2DElement of type Run
            // TODO: Implement translation for SettingsProF3BElement of type Run
            // TODO: Implement translation for SettingsProF3DElement of type Run
            // TODO: Implement translation for SettingsProF4BElement of type Run
            // TODO: Implement translation for SettingsProF4DElement of type Run
            // TODO: Implement translation for SettingsProF5BElement of type Run
            // TODO: Implement translation for SettingsProF5DElement of type Run
            // TODO: Implement translation for SettingsProF6BElement of type Run
            // TODO: Implement translation for SettingsProF6DElement of type Run
            // TODO: Implement translation for SettingsProF7BElement of type Run
            // TODO: Implement translation for SettingsProF7DElement of type Run
            // TODO: Implement translation for SettingsProF8BElement of type Run
            // TODO: Implement translation for SettingsProF8DElement of type Run
            // TODO: Implement translation for SettingsProF9BElement of type Run
            // TODO: Implement translation for SettingsProF9DElement of type Run
            SettingsProF10Element.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_ProF10");
            SettingsProF11Element.Text = Helpers.LocalizationHelper.Instance.GetString("Settings_ProF11");
            AuthProMonthlyTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProMonthlyTitle");
            AuthProMonthlySubtitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProMonthlySubtitle");
            AuthProYearlyTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProYearlyTitle");
            AuthProYearlySubtitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProYearlySubtitle");
            AuthProYearlyBadgeElement.Text = Helpers.LocalizationHelper.Instance.GetString("Auth_ProYearlyBadge");
        }
}

