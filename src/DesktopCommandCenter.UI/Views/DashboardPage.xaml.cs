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
    }

    private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Saudação dinâmica por horário
        var authService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetService(typeof(DesktopCommandCenter.Application.Interfaces.IAuthService)) as DesktopCommandCenter.Application.Interfaces.IAuthService;
        DesktopCommandCenter.Application.Interfaces.AuthUser? currentUser = null;
        if (authService != null)
        {
            currentUser = await authService.GetCurrentUserAsync();
        }
        
        string nameSuffix = "";
        if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Email))
        {
            var parts = currentUser.Email.Split('@');
            if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
            {
                string namePart = parts[0];
                namePart = char.ToUpper(namePart[0]) + (namePart.Length > 1 ? namePart.Substring(1) : "");
                nameSuffix = $", {namePart}";
            }
        }

        int hour = DateTime.Now.Hour;
        TxtGreeting.Text = hour switch
        {
            >= 5 and < 12  => $"Bom dia{nameSuffix} 👋",
            >= 12 and < 18 => $"Boa tarde{nameSuffix} 👋",
            _              => $"Boa noite{nameSuffix} 👋"
        };

        // Versão do app
        TxtVersion.Text = $"v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.1"}";

        // Badge do plano
        bool isPro = App.IsProUnlocked;
        TxtPlanBadge.Text = isPro ? "PRO ✨" : "Community";
        TxtProBadge.Text  = isPro ? "" : "Upgrade para PRO para desbloquear mais ferramentas";
        BtnUpgradePro.Visibility = isPro ? Visibility.Collapsed : Visibility.Visible;
        SeparatorStripe.Visibility = isPro ? Visibility.Collapsed : Visibility.Visible;
        BtnStripeCheckout.Visibility = isPro ? Visibility.Collapsed : Visibility.Visible;
    }

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

    private async void BtnUpgradePro_Click(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(
            new Uri("https://foreigntechnologies.com.br/projects/windows/desktop-command-center"));
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
            // Requer login primeiro
            var dialog = new ContentDialog
            {
                Title = "Requer Login",
                Content = "Você precisa fazer login no aplicativo antes de prosseguir com a assinatura para vincular seu perfil à licença.",
                CloseButtonText = "Entendi",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
            WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("AuthPage"));
            return;
        }

        string url = $"{baseUrl}?client_reference_id={user.Uid}";
        await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
    }
}
