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

    [ObservableProperty]
    private int _selectedTimeFormatIndex;

    [ObservableProperty]
    private int _selectedDateFormatIndex;

    public System.Collections.ObjectModel.ObservableCollection<HotkeyConfigItemViewModel> Hotkeys { get; } = new();

    public SettingsViewModel(DesktopCommandCenter.Application.Interfaces.IHotkeyConfigManager hotkeyManager)
    {
        _hotkeyManager = hotkeyManager;

        // Read saved theme
        string themeStr = App.GetTheme();
        if (themeStr == "Light") SelectedThemeIndex = 0;
        else if (themeStr == "Dark") SelectedThemeIndex = 1;
        else SelectedThemeIndex = 2; // Default / Contrast

        // Read saved format indexes
        string timeFormat = App.GetTimeFormat();
        SelectedTimeFormatIndex = timeFormat switch
        {
            "HH:mm" => 0,
            "HH:mm:ss" => 1,
            "hh:mm tt" => 2,
            "hh:mm:ss tt" => 3,
            _ => 0
        };

        string dateFormat = App.GetDateFormat();
        SelectedDateFormatIndex = dateFormat switch
        {
            "dddd, dd MMMM yyyy" => 0,
            "dd/MM/yyyy" => 1,
            "yyyy-MM-dd" => 2,
            "MMM d, yyyy" => 3,
            _ => 0
        };

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
        
        App.SaveTheme(themeStr);
        App.ApplyTheme(themeStr);
    }

    partial void OnSelectedTimeFormatIndexChanged(int value)
    {
        string format = value switch
        {
            0 => "HH:mm",
            1 => "HH:mm:ss",
            2 => "hh:mm tt",
            3 => "hh:mm:ss tt",
            _ => "HH:mm"
        };
        App.SaveTimeFormat(format);
    }

    partial void OnSelectedDateFormatIndexChanged(int value)
    {
        string format = value switch
        {
            0 => "dddd, dd MMMM yyyy",
            1 => "dd/MM/yyyy",
            2 => "yyyy-MM-dd",
            3 => "MMM d, yyyy",
            _ => "dddd, dd MMMM yyyy"
        };
        App.SaveDateFormat(format);
    }
}
