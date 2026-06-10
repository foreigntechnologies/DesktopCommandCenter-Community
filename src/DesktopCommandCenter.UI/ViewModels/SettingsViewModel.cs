using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using Windows.Storage;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private int _selectedThemeIndex;

    public SettingsViewModel()
    {
        // Ler tema salvo
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue("AppTheme", out object? themeObj) && themeObj is string themeStr)
        {
            if (themeStr == "Light") SelectedThemeIndex = 0;
            else if (themeStr == "Dark") SelectedThemeIndex = 1;
            else SelectedThemeIndex = 2; // Default / Contrast
        }
        else
        {
            SelectedThemeIndex = 2; // Padrão
        }
    }

    partial void OnSelectedThemeIndexChanged(int value)
    {
        string themeStr = value switch
        {
            0 => "Light",
            1 => "Dark",
            _ => "Default"
        };
        
        ApplicationData.Current.LocalSettings.Values["AppTheme"] = themeStr;

        // Apply immediately
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
            // Se for nulo por algum motivo, aplica globalmente
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
