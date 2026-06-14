using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AuthPage : Page
{
    public AuthViewModel ViewModel { get; }

    public AuthPage()
    {
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<AuthViewModel>();
        InitializeComponent();
        Loaded += AuthPage_Loaded;
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    private void AuthPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        UpdateTranslations();
    }

    private void UpdateTranslations()
    {
        var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        
        TxtAuthTitle.Text = loc.GetString("Auth_Title");
        TxtAuthDesc.Text = loc.GetString("Auth_Desc");
        TxtLoading.Text = loc.GetString("Auth_Loading");
        TxtGoogle.Text = loc.GetString("Auth_Google");
        TxtGitHub.Text = loc.GetString("Auth_GitHub");
        TxtTermsPrefix.Text = loc.GetString("Auth_TermsPrefix");
        HypTerms.Text = loc.GetString("Auth_Terms");
        TxtTermsMid.Text = loc.GetString("Auth_TermsMid");
        HypPrivacy.Text = loc.GetString("Auth_Privacy");
        TxtTermsSuffix.Text = loc.GetString("Auth_TermsSuffix");
        TxtLinkAccountsTitle.Text = loc.GetString("Auth_LinkTitle");
        TxtLinkGoogle.Text = loc.GetString("Auth_LinkGoogle");
        TxtLinkGitHub.Text = loc.GetString("Auth_LinkGitHub");
        TxtLinkDesc.Text = loc.GetString("Auth_LinkDesc");
        TxtProAdTitle.Text = loc.GetString("Auth_ProAdTitle");
        TxtProAd1.Text = loc.GetString("Auth_ProAd1");
        TxtProAd2.Text = loc.GetString("Auth_ProAd2");
        TxtProAd3.Text = loc.GetString("Auth_ProAd3");
        TxtProAd4.Text = loc.GetString("Auth_ProAd4");
        TxtProMonthly.Text = loc.GetString("Auth_ProMonthly");
        TxtProYearly.Text = loc.GetString("Auth_ProYearly");
        TxtProActiveTitle.Text = loc.GetString("Auth_ProActiveTitle");
        TxtProActiveDesc.Text = loc.GetString("Auth_ProActiveDesc");
        TxtProManage.Text = loc.GetString("Auth_ProManage");
        BtnLogout.Content = loc.GetString("Auth_Logout");
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

}
