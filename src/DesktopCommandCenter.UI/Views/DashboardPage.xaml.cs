using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Messaging;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage()
    {
        ViewModel = new DashboardViewModel();
        InitializeComponent();
        Loaded += DashboardPage_Loaded;
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    private void UpdateTranslations()
    {
        var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        TxtPro1Title.Text = loc.GetString("Dash_ProFeature1");
        TxtPro1Desc.Text = loc.GetString("Dash_ProFeature1Desc");
        TxtPro2Title.Text = loc.GetString("Dash_ProFeature2");
        TxtPro2Desc.Text = loc.GetString("Dash_ProFeature2Desc");
        TxtPro3Title.Text = loc.GetString("Dash_ProFeature4"); // Cloud Sync
        TxtPro3Desc.Text = loc.GetString("Dash_ProFeature4Desc");
        TxtPro4Title.Text = loc.GetString("Dash_ProFeature3"); // Perfis
        TxtPro4Desc.Text = loc.GetString("Dash_ProFeature3Desc");

        // Top Metrics
        TxtDashTools.Text = loc.GetString("Dash_Tools");
        TxtDashNotes.Text = loc.GetString("Dash_Notes");
        TxtDashClip.Text = loc.GetString("Dash_Clipboard");
        TxtDashPlan.Text = loc.GetString("Dash_Plan");

        // Sections
        TxtDashCommSection.Text = loc.GetString("Dash_CommSection");
        TxtDashProSection.Text = loc.GetString("Dash_ProSection");

        // Community Tools
        TxtToolColorTitle.Text = loc.GetString("Dash_ToolColorTitle");
        TxtToolColorDesc.Text = loc.GetString("Dash_ToolColorDesc");
        TxtToolClipTitle.Text = loc.GetString("Dash_ToolClipTitle");
        TxtToolClipDesc.Text = loc.GetString("Dash_ToolClipDesc");
        TxtToolAwakeTitle.Text = loc.GetString("Dash_ToolAwakeTitle");
        TxtToolAwakeDesc.Text = loc.GetString("Dash_ToolAwakeDesc");
        TxtToolCaptureTitle.Text = loc.GetString("Dash_ToolCaptureTitle");
        TxtToolCaptureDesc.Text = loc.GetString("Dash_ToolCaptureDesc");
        TxtToolTransTitle.Text = loc.GetString("Dash_ToolTransTitle");
        TxtToolTransDesc.Text = loc.GetString("Dash_ToolTransDesc");
        TxtToolTimerTitle.Text = loc.GetString("Dash_ToolTimerTitle");
        TxtToolTimerDesc.Text = loc.GetString("Dash_ToolTimerDesc");
        TxtToolUpdateTitle.Text = loc.GetString("Dash_ToolUpdateTitle");
        TxtToolUpdateDesc.Text = loc.GetString("Dash_ToolUpdateDesc");
        TxtToolSearchTitle.Text = loc.GetString("Dash_ToolSearchTitle");
        TxtToolSearchDesc.Text = loc.GetString("Dash_ToolSearchDesc");
        TxtToolPalleteTitle.Text = loc.GetString("Dash_ToolPalleteTitle");
        TxtToolPalleteDesc.Text = loc.GetString("Dash_ToolPalleteDesc");
        TxtToolShellTitle.Text = loc.GetString("Dash_ToolShellTitle");
        TxtToolShellDesc.Text = loc.GetString("Dash_ToolShellDesc");

        // PRO Features missing items
        TxtDashPromptsTitle.Text = loc.GetString("Dash_ToolPromptsTitle");
        TxtDashPromptsDesc.Text = loc.GetString("Dash_ToolPromptsDesc");

        // Footer links
        BtnDashDoc.Content = loc.GetString("Dash_DocButton");
        BtnDashProject.Content = loc.GetString("Dash_ProjectButton");
        BtnDashGit.Content = loc.GetString("Dash_GitButton");
        BtnDashBug.Content = loc.GetString("Dash_BugButton");
        TxtCopyright.Text = loc.GetString("Dash_Copyright");
    }

    private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTranslations();
        
        // Saudação dinâmica por horário
        var authService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetService(typeof(DesktopCommandCenter.Application.Interfaces.IAuthService)) as DesktopCommandCenter.Application.Interfaces.IAuthService;
        DesktopCommandCenter.Application.Interfaces.AuthUser? currentUser = null;
        if (authService != null)
        {
            currentUser = await authService.GetCurrentUserAsync();
        }
        
        string nameSuffix = "";
        if (currentUser != null)
        {
            // Usa o nome real do perfil (ex: Google OAuth "Gustavo Koglin")
            // Fallback para o prefixo do email se o DisplayName estiver vazio
            string nameToShow = !string.IsNullOrWhiteSpace(currentUser.DisplayName)
                ? currentUser.DisplayName
                : (!string.IsNullOrWhiteSpace(currentUser.Email)
                    ? CapitalizeFirst(currentUser.Email.Split('@')[0])
                    : "");

            if (!string.IsNullOrWhiteSpace(nameToShow))
                nameSuffix = $", {nameToShow}";
        }

        int hour = DateTime.Now.Hour;
        string greetingKey = hour switch
        {
            < 5 => "GreetingEvening",
            < 12 => "GreetingMorning",
            < 18 => "GreetingAfternoon",
            _ => "GreetingEvening"
        };
        var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        TxtGreeting.Text = loc.GetString(greetingKey) + nameSuffix;

        // Versão do app
        try
        {
            var v = Windows.ApplicationModel.Package.Current.Id.Version;
            TxtVersion.Text = $"v{v.Major}.{v.Minor}.{v.Build}";
        }
        catch
        {
            var attr = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false);
            if (attr.Length > 0)
            {
                var infoVer = ((System.Reflection.AssemblyInformationalVersionAttribute)attr[0]).InformationalVersion;
                TxtVersion.Text = $"v{infoVer.Split('+')[0]}";
            }
            else
            {
                TxtVersion.Text = "v1.0.0";
            }
        }

        // Badge do plano
        bool isPro = App.IsProUnlocked;
        TxtPlanBadge.Text = isPro ? "PRO ✨" : "Community";
        TxtProBadge.Text  = isPro ? "" : loc.GetString("Dash_ProBadge");
        BtnUpgradePro.Visibility = isPro ? Visibility.Collapsed : Visibility.Visible;
        SeparatorStripe.Visibility = isPro ? Visibility.Collapsed : Visibility.Visible;
        BtnStripeCheckout.Visibility = isPro ? Visibility.Collapsed : Visibility.Visible;
        SeparatorBugReport.Visibility = isPro ? Visibility.Collapsed : Visibility.Visible;
        
        await ViewModel.LoadMetricsAsync();
    }

    private static string CapitalizeFirst(string s)
        => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

    private void ToolCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string actionId)
            WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage(actionId));
    }

    private async void BtnBugReport_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new BugReportDialog { XamlRoot = this.XamlRoot };
        await dialog.ShowAsync();
    }

    private void BtnUpgradePro_Click(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("Auth"));
    }

    private async void StripeMonthly_Click(object sender, RoutedEventArgs e)
    {
        await OpenStripeCheckoutAsync("https://buy.stripe.com/14AeVf9Q46Gz5nY9ttf3a0p");
    }

    private async void StripeYearly_Click(object sender, RoutedEventArgs e)
    {
        await OpenStripeCheckoutAsync("https://buy.stripe.com/7sYbJ3e6k3uncQq499f3a0q");
    }

    private async System.Threading.Tasks.Task OpenStripeCheckoutAsync(string baseUrl)
    {
        var authService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetService(typeof(DesktopCommandCenter.Application.Interfaces.IAuthService)) as DesktopCommandCenter.Application.Interfaces.IAuthService;
        var user = authService != null ? await authService.GetCurrentUserAsync() : null;
        
        if (user == null || string.IsNullOrEmpty(user.Uid))
        {
            var t = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
            // Requer login primeiro
            var dialog = new ContentDialog
            {
                Title = t.GetString("Dialog_LoginRequired_Title") ?? "Requer Login",
                Content = t.GetString("Dialog_LoginRequired_Content") ?? "Você precisa fazer login no aplicativo antes de prosseguir com a assinatura para vincular seu perfil à licença.",
                CloseButtonText = t.GetString("Dialog_GotIt") ?? "Entendi",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
            WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("Auth"));
            return;
        }

        string url = $"{baseUrl}?client_reference_id={user.Uid}";
        await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
    }
}
