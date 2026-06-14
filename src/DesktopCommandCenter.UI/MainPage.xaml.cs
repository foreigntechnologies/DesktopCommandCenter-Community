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
        this.Loaded += MainPage_Loaded;
        
        // Removed broken x:Bind DataContext
        // AppNavigationView.DataContext = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        UpdateTranslations();

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

    private void UpdateTranslations()
    {
        var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;

        NavDashboard.Content = loc.GetString("Nav_Dashboard");
        NavProcessManager.Content = loc.GetString("Nav_ProcessManager");
        NavNotes.Content = loc.GetString("Nav_Notes");
        HeaderSysUtils.Content = loc.GetString("Nav_SysUtils");
        NavColorPicker.Content = loc.GetString("Nav_ColorPicker");
        NavAwake.Content = loc.GetString("Nav_Awake");
        NavCliCommands.Content = loc.GetString("Nav_CliCommands");
        NavClipboard.Content = loc.GetString("Nav_Clipboard");
        NavCalculator.Content = loc.GetString("Nav_Calculator");
        NavTimer.Content = loc.GetString("Nav_Timer");
        NavCapture.Content = loc.GetString("Nav_Capture");
        NavTranslator.Content = loc.GetString("Nav_Translator");
        HeaderPro.Content = loc.GetString("Nav_ProFeatures");
        NavIALocal.Content = loc.GetString("Nav_ChatFT");
        NavSearch.Content = loc.GetString("Nav_Search");
        NavPrompts.Content = loc.GetString("Nav_Prompts");
        NavAutomations.Content = loc.GetString("Nav_Automations");
        NavMarketplace.Content = loc.GetString("Nav_Marketplace");
        NavAuth.Content = loc.GetString("Nav_Auth");

        UpdateNavigationLocks();
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

        bool isProFeature = actionId == "IALocal" || actionId == "PesquisaUniversal" || actionId == "Prompts" || actionId == "Automacoes" || actionId == "Marketplace";
        if (isProFeature && !App.IsProUnlocked)
        {
            if (actionId == "IALocal" && !string.IsNullOrEmpty(App.GetAIAgentApiKey()))
            {
                // Permitido se tiver chave de API própria
            }
            else
            {
                ShowProRequiredDialog(actionId);
                return;
            }
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
                "ProcessManager" => typeof(Views.ProcessManagerPage),
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
                if (tag == "IALocal" && !string.IsNullOrEmpty(App.GetAIAgentApiKey()))
                {
                    // Permitido se tiver chave própria
                }
                else
                {
                    ShowProRequiredDialog(tag ?? "Recurso PRO");
                    return;
                }
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
        var t = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;

        var dialog = new ContentDialog
        {
            Title = t.GetString("Dialog_ProRequired_Title") ?? "Recurso PRO Necessário",
            Content = isLoggedIn 
                ? string.Format(t.GetString("Dialog_ProRequired_Content_LoggedIn") ?? "O recurso '{0}' está disponível apenas para assinantes PRO. Assine agora para liberar!", featureTag)
                : string.Format(t.GetString("Dialog_ProRequired_Content_LoggedOut") ?? "O recurso '{0}' está disponível apenas para assinantes PRO. Faça login ou assine agora para liberar!", featureTag),
            PrimaryButtonText = isLoggedIn 
                ? (t.GetString("Dialog_ProRequired_Primary_LoggedIn") ?? "Assinar") 
                : (t.GetString("Dialog_ProRequired_Primary_LoggedOut") ?? "Entrar / Assinar"),
            CloseButtonText = t.GetString("Dialog_Cancel") ?? "Cancelar",
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
                
                bool isLocked = System.Array.Exists(proFeatures, f => f == tag) && !App.IsProUnlocked;
                if (isLocked && tag == "IALocal" && !string.IsNullOrEmpty(App.GetAIAgentApiKey()))
                {
                    isLocked = false; // Desbloqueia se tiver chave própria
                }

                if (isLocked)
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

    private async void MainPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var dir = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "DCC");
        var filePath = System.IO.Path.Combine(dir, "dcc_app_language.txt");
        if (!System.IO.File.Exists(filePath))
        {
            FirstLaunchLanguageDialog.XamlRoot = this.XamlRoot;
            await FirstLaunchLanguageDialog.ShowAsync();
        }
    }

    private void CmbFirstLaunchLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FirstLaunchLanguageDialog.IsPrimaryButtonEnabled = CmbFirstLaunchLanguage.SelectedItem != null;
    }

    private void FirstLaunchLanguageDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (CmbFirstLaunchLanguage.SelectedItem is ComboBoxItem item && item.Tag is string lang)
        {
            App.SaveAppLanguage(lang);
            var tService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ITranslationService>((App.Current as App).Services);
            _ = tService.SetLanguageAsync(lang);
            UpdateTranslations();
            
            // Also force update the dialog UI itself using LocalizationHelper
            var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
            FirstLaunchLanguageDialog.Title = loc.GetString("Settings_Language"); 
        }
    }
}
