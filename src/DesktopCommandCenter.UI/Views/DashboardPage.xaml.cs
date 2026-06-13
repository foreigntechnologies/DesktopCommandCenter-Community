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
        var userName = currentUser?.DisplayName;
        string nameSuffix = string.IsNullOrWhiteSpace(userName) ? "" : $", {userName.Split(' ')[0]}";

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
            new Uri("https://foreigntechnologies.com.br/dcc/pro"));
    }
}
