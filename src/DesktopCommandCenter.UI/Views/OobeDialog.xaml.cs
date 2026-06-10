using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class OobeDialog : ContentDialog
{
    public OobeDialog()
    {
        this.InitializeComponent();
        
        this.PrimaryButtonClick += OobeDialog_PrimaryButtonClick;
    }

    private void OobeDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        string themeStr = ThemeComboBox.SelectedIndex switch
        {
            0 => "Light",
            1 => "Dark",
            _ => "Default"
        };
        
        ApplicationData.Current.LocalSettings.Values["AppTheme"] = themeStr;
        
        if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = themeStr switch
            {
                "Light" => Microsoft.UI.Xaml.ElementTheme.Light,
                "Dark" => Microsoft.UI.Xaml.ElementTheme.Dark,
                _ => Microsoft.UI.Xaml.ElementTheme.Default
            };
        }
        else
        {
            var mainWindow = App.Current.GetType().GetField("_window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(App.Current) as Microsoft.UI.Xaml.Window;
            if (mainWindow?.Content is Microsoft.UI.Xaml.FrameworkElement fw)
            {
                fw.RequestedTheme = themeStr switch
                {
                    "Light" => Microsoft.UI.Xaml.ElementTheme.Light,
                    "Dark" => Microsoft.UI.Xaml.ElementTheme.Dark,
                    _ => Microsoft.UI.Xaml.ElementTheme.Default
                };
            }
        }
    }
}
