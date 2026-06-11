using Microsoft.UI.Xaml.Controls;
using System;
using CommunityToolkit.Mvvm.Messaging;

namespace DesktopCommandCenter.UI;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        
        AppNavigationView.DataContext = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        
        // Navigate to Dashboard by default
        ContentFrame.Navigate(typeof(Views.DashboardPage));
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

    private void NavigateToAction(string actionId)
    {
        if (actionId == "Settings")
        {
            ContentFrame.Navigate(typeof(Views.SettingsPage));
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

        if (pageType != null)
        {
            ContentFrame.Navigate(pageType, actionId);
            
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
        if (args.IsSettingsInvoked)
        {
            ContentFrame.Navigate(typeof(Views.SettingsPage));
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

            if (pageType != null)
            {
                ContentFrame.Navigate(pageType, tag);
            }
        }
    }

    private async void ShowProRequiredDialog(string featureTag)
    {
        var dialog = new ContentDialog
        {
            Title = "Recurso PRO Necessário",
            Content = $"O recurso '{featureTag}' está disponível apenas para assinantes PRO. Assine agora para liberar!",
            PrimaryButtonText = "Assinar PRO",
            CloseButtonText = "Cancelar",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Simulate subscription logic
            await SimulateProInstallation();
        }
        else
        {
            // Revert selection
            AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
        }
    }

    private async System.Threading.Tasks.Task SimulateProInstallation()
    {
        if (!App.IsProBuild)
        {
            // Community build needs to simulate downloading/installing modules
            var progressDialog = new ContentDialog
            {
                Title = "Instalando Módulos PRO",
                Content = new ProgressRing { IsActive = true, Width = 50, Height = 50, Margin = new Microsoft.UI.Xaml.Thickness(0, 20, 0, 0) },
                XamlRoot = this.XamlRoot
            };

            var showTask = progressDialog.ShowAsync();
            await System.Threading.Tasks.Task.Delay(3000); // Simulate installation time
            progressDialog.Hide();
        }

        App.IsProUnlocked = true;
        UpdateNavigationLocks();
        
        // Navigate to the Dashboard or refresh to show unlock success
        ContentFrame.Navigate(typeof(Views.DashboardPage));
        AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
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
