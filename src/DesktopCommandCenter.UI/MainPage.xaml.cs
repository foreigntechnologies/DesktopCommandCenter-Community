using Microsoft.UI.Xaml.Controls;
using System;
using CommunityToolkit.Mvvm.Messaging;

namespace DesktopCommandCenter.UI;

public sealed partial class MainPage : Page
{
    private DateTime _lastNavTime = DateTime.MinValue;

    public MainPage()
    {
        InitializeComponent();
        
        AppNavigationView.DataContext = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        
        // Navigate to Dashboard by default
        ContentFrame.Navigate(typeof(Views.DashboardPage), null, new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
        AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
        
        UpdateNavigationLocks();

        WeakReferenceMessenger.Default.Register<Messages.NavigateMessage>(this, (r, m) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                NavigateToAction(m.Value);
            });
        });

        WeakReferenceMessenger.Default.Register<Messages.LicenseChangedMessage>(this, (r, m) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateNavigationLocks();
            });
        });

        _ = ValidateProLicenseAsync();
    }

    private async System.Threading.Tasks.Task ValidateProLicenseAsync()
    {
        try
        {
            var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAuthService>((App.Current as App).Services);
            var licenseService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ILicenseService>((App.Current as App).Services);

            var user = await authService.GetCurrentUserAsync();
            if (user != null)
            {
                var plan = await licenseService.GetCurrentPlanAsync();
                App.IsProUnlocked = App.IsProBuild && plan.Equals("pro", StringComparison.OrdinalIgnoreCase);
                WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(App.IsProUnlocked));
            }
        }
        catch { }
    }

    private void NavigateToAction(string actionId)
    {
        if ((DateTime.Now - _lastNavTime).TotalMilliseconds < 350) return;
        _lastNavTime = DateTime.Now;

        if (actionId == "Settings")
        {
            if (ContentFrame.CurrentSourcePageType != typeof(Views.SettingsPage))
            {
                ContentFrame.Navigate(typeof(Views.SettingsPage), null, new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
            AppNavigationView.SelectedItem = AppNavigationView.SettingsItem;
            return;
        }

        Type? pageType = actionId switch
        {
            "Dashboard" => typeof(Views.DashboardPage),
            "Notes" => typeof(Views.NotesPage),
            "Clipboard" => typeof(Views.ClipboardPage),
            "Calculadora" => typeof(Views.CalculadoraPage),
            "ColorPicker" => typeof(Views.ColorPickerPage),
            "Awake" => typeof(Views.AwakePage),
            "AlwaysOnTop" => typeof(Views.AlwaysOnTopPage),
            "Temporizador" => typeof(Views.TemporizadorPage),
            "Captura" => typeof(Views.CapturaPage),
            "Tradutor" => typeof(Views.TradutorPage),
            "IALocal" => typeof(Views.IALocalPage),
            "PesquisaUniversal" => typeof(Views.PesquisaUniversalPage),
            "Prompts" => typeof(Views.PromptsPage),
            "Automacoes" => typeof(Views.AutomacoesPage),
            "Auth" => typeof(Views.AuthPage),
            "CliCommands" => typeof(Views.CliCommandsPage),
            _ => typeof(Views.ComingSoonPage)
        };

        if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
        {
            ContentFrame.Navigate(pageType, actionId, new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            
            foreach (var item in AppNavigationView.MenuItems)
            {
                if (item is NavigationViewItem navItem && navItem.Tag?.ToString() == actionId)
                {
                    AppNavigationView.SelectedItem = navItem;
                    break;
                }
            }
        }
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if ((DateTime.Now - _lastNavTime).TotalMilliseconds < 350) return;
        _lastNavTime = DateTime.Now;

        if (args.IsSettingsInvoked)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(Views.SettingsPage))
            {
                ContentFrame.Navigate(typeof(Views.SettingsPage), null, new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
        }
        else if (args.InvokedItemContainer != null)
        {
            var tag = args.InvokedItemContainer.Tag?.ToString();

            Type? pageType = tag switch
            {
                "Dashboard" => typeof(Views.DashboardPage),
                "Notes" => typeof(Views.NotesPage),
                "Clipboard" => typeof(Views.ClipboardPage),
                "Calculadora" => typeof(Views.CalculadoraPage),
                "ColorPicker" => typeof(Views.ColorPickerPage),
                "Awake" => typeof(Views.AwakePage),
                "AlwaysOnTop" => typeof(Views.AlwaysOnTopPage),
                "Temporizador" => typeof(Views.TemporizadorPage),
                "Captura" => typeof(Views.CapturaPage),
                "Tradutor" => typeof(Views.TradutorPage),
                "IALocal" => typeof(Views.IALocalPage),
                "PesquisaUniversal" => typeof(Views.PesquisaUniversalPage),
                "Prompts" => typeof(Views.PromptsPage),
                "Automacoes" => typeof(Views.AutomacoesPage),
                "Auth" => typeof(Views.AuthPage),
                "CliCommands" => typeof(Views.CliCommandsPage),
                _ => typeof(Views.ComingSoonPage)
            };

            bool isProFeature = tag == "IALocal" || tag == "PesquisaUniversal" || tag == "Prompts" || tag == "Automacoes" || tag == "Marketplace";

            if (isProFeature && !App.IsProUnlocked)
            {
                ShowProRequiredDialog(tag ?? "Recurso PRO");
                return;
            }

            if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
            {
                ContentFrame.Navigate(pageType, tag, new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
        }
    }

    private async void ShowProRequiredDialog(string featureTag)
    {
        var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAuthService>((App.Current as App).Services);
        bool isLoggedIn = authService.IsAuthenticated;

        var dialog = new ContentDialog
        {
            Title = "Recurso PRO Necessário",
            Content = isLoggedIn 
                ? $"O recurso '{featureTag}' está disponível apenas para assinantes PRO. Assine agora para liberar!"
                : $"O recurso '{featureTag}' está disponível apenas para assinantes PRO. Faça login ou assine agora para liberar!",
            PrimaryButtonText = isLoggedIn ? "Assinar" : "Entrar / Assinar",
            CloseButtonText = "Cancelar",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Redireciona para a tela de Auth ao invés de simular a compra local
            NavigateToAction("Auth");
        }
        else
        {
            // Revert selection
            AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
        }
    }

    public void UpdateNavigationLocks()
    {
        var proFeatures = new[] { "IALocal", "PesquisaUniversal", "Prompts", "Automacoes", "Marketplace" };
        foreach (var item in AppNavigationView.MenuItems)
        {
            if (item is NavigationViewItem navItem)
            {
                var tag = navItem.Tag?.ToString();
                if (string.IsNullOrEmpty(tag)) continue;
                
                if (System.Array.Exists(proFeatures, f => f == tag) && !App.IsProUnlocked)
                {
                    // Locked state
                    if (navItem.Content != null)
                    {
                        navItem.Content = navItem.Content.ToString()?.Replace(" 🔒", "") + " 🔒";
                    }
                }
                else
                {
                    // Unlocked state
                    if (navItem.Content != null)
                    {
                        navItem.Content = navItem.Content.ToString()?.Replace(" 🔒", "");
                    }
                }
            }
        }
    }
}
