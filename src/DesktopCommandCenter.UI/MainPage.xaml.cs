using Microsoft.UI.Xaml.Controls;
using System;
using CommunityToolkit.Mvvm.Messaging;

namespace DesktopCommandCenter.UI;

public sealed partial class MainPage : Page
{
    private DateTime _lastNavTime = DateTime.MinValue;
    private Microsoft.UI.Xaml.DispatcherTimer? _timer;
    private ulong _prevIdleTime;
    private ulong _prevKernelTime;
    private ulong _prevUserTime;

    public MainPage()
    {
        InitializeComponent();
        this.Loaded += MainPage_Loaded;
        this.Unloaded += MainPage_Unloaded;
        
        // Removed broken x:Bind DataContext
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

    }

    private void UpdateTranslations()
    {
        var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;

        NavDashboard.Content = loc.GetString("Nav_Dashboard");
        NavProcessManager.Content = loc.GetString("Nav_ProcessManager");
        NavNotes.Content = loc.GetString("Nav_Notes");
        NavColorPicker.Content = loc.GetString("Nav_ColorPicker");
        NavAwake.Content = loc.GetString("Nav_Awake");
        NavClipboard.Content = loc.GetString("Nav_Clipboard");



        NavTimer.Content = loc.GetString("Nav_Timer");
        NavCapture.Content = loc.GetString("Nav_Capture");
        NavTranslator.Content = loc.GetString("Nav_Translator");
        NavIALocal.Content = loc.GetString("Nav_ChatFT");
        NavApps.Content = loc.GetString("Nav_Apps");

        if (ChatFTHeaderButton != null)
        {
            var tooltip = new Microsoft.UI.Xaml.Controls.ToolTip();
            tooltip.Content = loc.GetString("AskAITooltip");
            Microsoft.UI.Xaml.Controls.ToolTipService.SetToolTip(ChatFTHeaderButton, tooltip);
        }
        NavSearch.Content = loc.GetString("Nav_Search");
        NavPrompts.Content = loc.GetString("Nav_Prompts");
        NavAutomations.Content = loc.GetString("Nav_Automations");
        NavMarketplace.Content = loc.GetString("Nav_Marketplace");
        NavAuth.Content = loc.GetString("Nav_Auth");

        if (AppNavigationView.SettingsItem is Microsoft.UI.Xaml.Controls.NavigationViewItem navSettingsItem)
        {
            navSettingsItem.Content = loc.GetString("Nav_Settings");
        }

        UpdateNavigationLocks();
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
        else if (actionId == "ManageHotkeys")
        {
            ContentFrame.Navigate(typeof(Views.SettingsPage), "OpenHotkeysDialog", new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            AppNavigationView.SelectedItem = AppNavigationView.SettingsItem;
            return;
        }
        else if (actionId == "FutureShell" || actionId == "IALocal")
        {
            if (actionId == "IALocal")
            {
                if (!App.IsProUnlocked && string.IsNullOrEmpty(App.GetAIAgentApiKey()))
                {
                    ShowProRequiredDialog("IALocal");
                    return;
                }
            }

            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            if (!string.IsNullOrEmpty(exePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = actionId == "IALocal" ? "--chatft" : "--futureshell",
                    UseShellExecute = true,
                    WorkingDirectory = System.IO.Path.GetDirectoryName(exePath)
                });
            }
            AppNavigationView.SelectedItem = null;
            return;
        }

        bool isProFeature = actionId == "Prompts" || actionId == "Automacoes" || actionId == "Marketplace";
        if (isProFeature && !App.IsProUnlocked)
        {
            ShowProRequiredDialog(actionId);
            return;
        }

        Type? pageType = actionId switch
        {
            "Dashboard" => typeof(Views.DashboardPage),
            "Notes" => typeof(Views.NotesPage),
            "Clipboard" => typeof(Views.ClipboardPage),

            "ColorPicker" => typeof(Views.ColorPickerPage),
            "Awake" => typeof(Views.AwakePage),
            "AlwaysOnTop" => typeof(Views.AlwaysOnTopPage),
            "Temporizador" => typeof(Views.TemporizadorPage),
            "SystemUpdates" => typeof(Views.SystemUpdatesPage),
            "Captura" => typeof(Views.CapturaPage),
            "Tradutor" => typeof(Views.TradutorPage),
            "PesquisaUniversal" => typeof(Views.PesquisaUniversalPage),
            "Prompts" => typeof(Views.PromptsPage),
            "Automacoes" => typeof(Views.AutomacoesPage),
            "Auth" => typeof(Views.AuthPage),
            "CliCommands" => typeof(Views.CliCommandsPage),
            "ProcessManager" => typeof(Views.ProcessManagerPage),
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

            if (tag == "FutureShell" || tag == "IALocal")
            {
                if (tag == "IALocal")
                {
                    if (!App.IsProUnlocked && string.IsNullOrEmpty(App.GetAIAgentApiKey()))
                    {
                        ShowProRequiredDialog("IALocal");
                        sender.SelectedItem = null;
                        return;
                    }
                }

                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = tag == "IALocal" ? "--chatft" : "--futureshell",
                        UseShellExecute = true,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(exePath)
                    });
                }
                sender.SelectedItem = null;
                return;
            }

            Type? pageType = tag switch
            {
                "Dashboard" => typeof(Views.DashboardPage),
                "AppsWorkspaces" => typeof(Views.AppsWorkspacesPage),
                "ProcessManager" => typeof(Views.ProcessManagerPage),
                "Notes" => typeof(Views.NotesPage),
                "Clipboard" => typeof(Views.ClipboardPage),

                "ColorPicker" => typeof(Views.ColorPickerPage),
                "Awake" => typeof(Views.AwakePage),
                "AlwaysOnTop" => typeof(Views.AlwaysOnTopPage),
                "Temporizador" => typeof(Views.TemporizadorPage),
                "System" => typeof(Views.SystemUpdatesPage),
                "Captura" => typeof(Views.CapturaPage),
                "Tradutor" => typeof(Views.TradutorPage),
                "PesquisaUniversal" => typeof(Views.PesquisaUniversalPage),
                "Prompts" => typeof(Views.PromptsPage),
                "Automacoes" => typeof(Views.AutomacoesPage),
                "Auth" => typeof(Views.AuthPage),
                "CliCommands" => typeof(Views.CliCommandsPage),
                "DeveloperHub" => typeof(Views.DeveloperHubPage),
                "Settings" => typeof(Views.SettingsPage),
                _ => typeof(Views.ComingSoonPage)
            };

        bool isProFeature = tag == "Prompts" || tag == "Automacoes" || tag == "Marketplace";

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
        var proFeatures = new[] { "IALocal", "Prompts", "Automacoes", "Marketplace" };
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
        UpdateTranslations(); // Garante que o SettingsItem seja traduzido, pois no construtor ele pode não estar instanciado ainda

        var dir = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "DCC");
        var filePath = System.IO.Path.Combine(dir, "dcc_app_language.txt");
        if (!System.IO.File.Exists(filePath))
        {
            FirstLaunchLanguageDialog.XamlRoot = this.XamlRoot;
            await FirstLaunchLanguageDialog.ShowAsync();
        }

        // Start real-time update timer for status bar
        _timer = new Microsoft.UI.Xaml.DispatcherTimer();
        _timer.Interval = System.TimeSpan.FromSeconds(1.5);
        _timer.Tick += Timer_Tick!;
        _timer.Start();
        Timer_Tick(this, null!);
    }

    private void MainPage_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick!;
            _timer = null;
        }
    }

    private void Timer_Tick(object? sender, object? e)
    {
        try
        {
            var memStatus = new DesktopCommandCenter.UI.Views.MEMORYSTATUSEX();
            memStatus.Init();
            if (DesktopCommandCenter.UI.Views.NativeMethods.GlobalMemoryStatusEx(ref memStatus) && StatusTextRam != null)
            {
                StatusTextRam.Text = $"RAM: {memStatus.dwMemoryLoad}%";
            }

            if (DesktopCommandCenter.UI.Views.NativeMethods.GetSystemTimes(out var idle, out var kernel, out var user) && StatusTextCpu != null)
            {
                ulong curIdle = idle.ToULong();
                ulong curKernel = kernel.ToULong();
                ulong curUser = user.ToULong();

                if (_prevIdleTime != 0)
                {
                    ulong sysDiff = (curKernel - _prevKernelTime) + (curUser - _prevUserTime);
                    ulong idleDiff = curIdle - _prevIdleTime;
                    
                    if (sysDiff > 0)
                    {
                        double cpuPct = (sysDiff - idleDiff) * 100.0 / sysDiff;
                        if (cpuPct < 0) cpuPct = 0;
                        if (cpuPct > 100) cpuPct = 100;
                        StatusTextCpu.Text = $"CPU: {cpuPct:F0}%";
                    }
                }
                
                _prevIdleTime = curIdle;
                _prevKernelTime = curKernel;
                _prevUserTime = curUser;
            }
        }
        catch { }
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

    private void ChatFTHeaderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        NavigateToAction("IALocal");
    }

    private void FutureShellHeaderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        NavigateToAction("FutureShell");
    }

    private void AppNavigationView_PaneOpening(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
    {
        try
        {
            var toggleButton = FindVisualChild<Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(sender, "TogglePaneButton");
            if (toggleButton != null)
            {
                // Substitui o ícone para um 'X' (Cancel) de forma segura (string em vez de objeto)
                toggleButton.Content = "\uE711"; 
            }
        }
        catch { }
    }

    private void AppNavigationView_PaneClosing(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewPaneClosingEventArgs args)
    {
        try
        {
            var toggleButton = FindVisualChild<Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(sender, "TogglePaneButton");
            if (toggleButton != null)
            {
                // Volta para o ícone Hamburguer (GlobalNavButton)
                toggleButton.Content = "\uE700"; 
            }
        }
        catch { }
    }

    private T? FindVisualChild<T>(Microsoft.UI.Xaml.DependencyObject parent, string name) where T : Microsoft.UI.Xaml.FrameworkElement
    {
        for (int i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T element && element.Name == name)
            {
                return element;
            }
            var result = FindVisualChild<T>(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
