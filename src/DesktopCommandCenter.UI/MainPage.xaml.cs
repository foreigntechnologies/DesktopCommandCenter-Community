using Microsoft.UI.Xaml.Controls;
using System;

namespace DesktopCommandCenter.UI;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        
        AppNavigationView.DataContext = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        
        // Navegar para a Dashboard por padrão
        ContentFrame.Navigate(typeof(Views.DashboardPage));
        AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
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
