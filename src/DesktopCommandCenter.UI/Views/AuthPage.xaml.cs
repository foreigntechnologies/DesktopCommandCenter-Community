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
        var loc = Helpers.LocalizationHelper.Instance;
        TxtAuthTitle.Text = loc.GetString("Auth_PageTitle");
        TxtAuthDesc.Text = loc.GetString("Auth_PageDesc");
        TxtLoading.Text = loc.GetString("Auth_TxtLoading");
        if (AuthBtnCancelElement.Content is string || AuthBtnCancelElement.Content == null) AuthBtnCancelElement.Content = loc.GetString("Auth_BtnCancel");
        
        // Botões sociais
        if (TxtGoogle != null) TxtGoogle.Text = loc.GetString("Auth_LoginGoogle");
        if (TxtGitHub != null) TxtGitHub.Text = loc.GetString("Auth_LoginGitHub");
        if (TxtMicrosoft != null) TxtMicrosoft.Text = loc.GetString("Auth_LoginMicrosoft");
        if (TxtLinkGoogle != null) TxtLinkGoogle.Text = loc.GetString("Auth_LinkGoogle");
        if (TxtLinkGitHub != null) TxtLinkGitHub.Text = loc.GetString("Auth_LinkGitHub");
        if (TxtLinkMicrosoft != null) TxtLinkMicrosoft.Text = loc.GetString("Auth_LinkMicrosoft");
        if (TxtLinkDesc != null) TxtLinkDesc.Text = loc.GetString("Auth_LinkDesc");
        
        TxtProActiveTitle.Text = loc.GetString("Auth_ProActiveTitle");
        TxtProActiveDesc.Text = loc.GetString("Auth_ProActiveDesc");
        TxtProManage.Text = loc.GetString("Auth_ProManage");
        TxtPausedActiveTitle.Text = loc.GetString("Auth_PausedTitle");
        TxtPausedActiveDesc.Text = loc.GetString("Auth_PausedDesc");
        TxtPausedManage.Text = loc.GetString("Auth_PausedManage");
        
        if (BtnLogout.Content is string || BtnLogout.Content == null) BtnLogout.Content = loc.GetString("Auth_BtnLogout");
        SettingsSubTitleElement.Text = loc.GetString("Settings_SubTitle");

        // NOVO LAYOUT - Traduções:
        if (SettingsCommHeader != null) SettingsCommHeader.Text = loc.GetString("Settings_CommHeader");
        if (SettingsCommBubble != null) SettingsCommBubble.Text = loc.GetString("Settings_CommBubble");
        if (SettingsProHeader != null) SettingsProHeader.Text = loc.GetString("Settings_ProHeader");
        if (SettingsProDescNew != null) SettingsProDescNew.Text = loc.GetString("Settings_ProDescNew");
        if (AuthRadioMonthly != null) AuthRadioMonthly.Text = loc.GetString("Auth_RadioMonthly");
        if (AuthRadioYearly != null) AuthRadioYearly.Text = loc.GetString("Auth_RadioYearly");
        if (AuthDiscountBadge != null) AuthDiscountBadge.Text = loc.GetString("Auth_DiscountBadge");

        // Funcionalidades
        if (SettingsCommDescElement != null) SettingsCommDescElement.Text = loc.GetString("Settings_CommDesc");
        if (SettingsCommF1BElement != null) SettingsCommF1BElement.Text = loc.GetString("Settings_CommF1B");
        if (SettingsCommF1DElement != null) SettingsCommF1DElement.Text = loc.GetString("Settings_CommF1D");
        if (SettingsCommF2BElement != null) SettingsCommF2BElement.Text = loc.GetString("Settings_CommF2B");
        if (SettingsCommF2DElement != null) SettingsCommF2DElement.Text = loc.GetString("Settings_CommF2D");
        if (SettingsCommF3BElement != null) SettingsCommF3BElement.Text = loc.GetString("Settings_CommF3B");
        if (SettingsCommF3DElement != null) SettingsCommF3DElement.Text = loc.GetString("Settings_CommF3D");
        if (SettingsCommF4BElement != null) SettingsCommF4BElement.Text = loc.GetString("Settings_CommF4B");
        if (SettingsCommF4DElement != null) SettingsCommF4DElement.Text = loc.GetString("Settings_CommF4D");
        if (SettingsCommF5BElement != null) SettingsCommF5BElement.Text = loc.GetString("Settings_CommF5B");
        if (SettingsCommF5DElement != null) SettingsCommF5DElement.Text = loc.GetString("Settings_CommF5D");
        
        if (SettingsCommF12Element != null) SettingsCommF12Element.Text = loc.GetString("Settings_CommF12");
        if (SettingsCommF13Element != null) SettingsCommF13Element.Text = loc.GetString("Settings_CommF13");
        if (SettingsCommF14Element != null) SettingsCommF14Element.Text = loc.GetString("Settings_CommF14");

        if (SettingsProF1Element != null) SettingsProF1Element.Text = loc.GetString("Settings_ProF1");
        if (SettingsProF2BElement != null) SettingsProF2BElement.Text = loc.GetString("Settings_ProF2B");
        if (SettingsProF2DElement != null) SettingsProF2DElement.Text = loc.GetString("Settings_ProF2D");
        if (SettingsProF3BElement != null) SettingsProF3BElement.Text = loc.GetString("Settings_ProF3B");
        if (SettingsProF3DElement != null) SettingsProF3DElement.Text = loc.GetString("Settings_ProF3D");
        if (SettingsProF4BElement != null) SettingsProF4BElement.Text = loc.GetString("Settings_ProF4B");
        if (SettingsProF4DElement != null) SettingsProF4DElement.Text = loc.GetString("Settings_ProF4D");
        if (SettingsProF5BElement != null) SettingsProF5BElement.Text = loc.GetString("Settings_ProF5B");
        if (SettingsProF5DElement != null) SettingsProF5DElement.Text = loc.GetString("Settings_ProF5D");
        
        if (SettingsProF6Element != null) SettingsProF6Element.Text = loc.GetString("Settings_ProF6Element");
        if (SettingsProF7Element != null) SettingsProF7Element.Text = loc.GetString("Settings_ProF7Element");
        if (SettingsProF10Element != null) SettingsProF10Element.Text = loc.GetString("Settings_ProF10");
    }
}


