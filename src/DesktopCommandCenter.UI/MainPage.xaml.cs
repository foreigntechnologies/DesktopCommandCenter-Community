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

        WeakReferenceMessenger.Default.Register<Messages.NavigateMessage>(this, (r, m) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                NavigateToAction(m.Value);
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
            "ColorPicker" => typeof(Views.ColorPickerPage),
            "Auth" => typeof(Views.AuthPage),
            "CliCommands" => typeof(Views.CliCommandsPage), // Will be created soon
            _ => null
        };

        if (pageType != null)
        {
            ContentFrame.Navigate(pageType);
            
            // Update menu selection
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
                "ColorPicker" => typeof(Views.ColorPickerPage),
                "Auth" => typeof(Views.AuthPage),
                _ => null
            };

            if (pageType != null)
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }
}
