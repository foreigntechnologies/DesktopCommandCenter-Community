using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using Windows.Storage;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly DesktopCommandCenter.Application.Interfaces.IHotkeyConfigManager _hotkeyManager;

    [ObservableProperty]
    private int _selectedThemeIndex;

    public System.Collections.ObjectModel.ObservableCollection<HotkeyConfigItemViewModel> Hotkeys { get; } = new();

    public SettingsViewModel(DesktopCommandCenter.Application.Interfaces.IHotkeyConfigManager hotkeyManager)
    {
        _hotkeyManager = hotkeyManager;

        // Read saved theme
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue("AppTheme", out object? themeObj) && themeObj is string themeStr)
        {
            if (themeStr == "Light") SelectedThemeIndex = 0;
            else if (themeStr == "Dark") SelectedThemeIndex = 1;
            else SelectedThemeIndex = 2; // Default / Contrast
        }
        else
        {
            SelectedThemeIndex = 2; // Default
        }

        LoadHotkeys();
    }

    private void LoadHotkeys()
    {
        Hotkeys.Clear();
        foreach (var config in _hotkeyManager.GetAllConfigs())
        {
            Hotkeys.Add(new HotkeyConfigItemViewModel
            {
                ActionId = config.ActionId,
                DisplayName = config.DisplayName,
                Modifiers = config.Modifiers,
                VirtualKey = config.VirtualKey,
                CurrentHotkeyDisplay = DesktopCommandCenter.Infrastructure.Services.GlobalHotkeyService.GetHotkeyString(config.Modifiers, config.VirtualKey)
            });
        }
    }

    public void SaveHotkey(HotkeyConfigItemViewModel item, uint modifiers, uint virtualKey)
    {
        _hotkeyManager.SaveConfig(item.ActionId, modifiers, virtualKey);
        item.Modifiers = modifiers;
        item.VirtualKey = virtualKey;
        item.CurrentHotkeyDisplay = DesktopCommandCenter.Infrastructure.Services.GlobalHotkeyService.GetHotkeyString(modifiers, virtualKey);
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
