using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Windows.Storage;

namespace DesktopCommandCenter.Infrastructure.Services;

public class HotkeyConfigManager : IHotkeyConfigManager
{
    private const string SettingsKey = "DccGlobalHotkeys";
    private List<HotkeyConfig> _configs = new();

    public event EventHandler? ConfigsChanged;

    public HotkeyConfigManager()
    {
        LoadConfigs();
    }

    private void LoadConfigs()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue(SettingsKey, out var value) && value is string json)
        {
            try
            {
                var loaded = JsonSerializer.Deserialize<List<HotkeyConfig>>(json);
                if (loaded != null)
                {
                    _configs = loaded;
                    EnsureDefaults();
                    return;
                }
            }
            catch { }
        }

        EnsureDefaults();
    }

    private void EnsureDefaults()
    {
        var defaults = new List<HotkeyConfig>
        {
            new HotkeyConfig { ActionId = "ColorPicker", DisplayName = "Color Picker", Modifiers = 0, VirtualKey = 0 }, // User configurable
            new HotkeyConfig { ActionId = "Settings", DisplayName = "Configurações", Modifiers = 2, VirtualKey = 0xDE }, // 2 = Ctrl, 0xDE = Oem7 (quote key in US layout)
            new HotkeyConfig { ActionId = "Dashboard", DisplayName = "Dashboard", Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Notes", DisplayName = "Notes", Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Clipboard", DisplayName = "Smart Clipboard", Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "CliCommands", DisplayName = "Comandos CLI", Modifiers = 0, VirtualKey = 0 }
        };

        bool changed = false;
        foreach (var def in defaults)
        {
            if (!_configs.Any(c => c.ActionId == def.ActionId))
            {
                _configs.Add(def);
                changed = true;
            }
        }

        if (changed) SaveToSettings();
    }

    private void SaveToSettings()
    {
        var json = JsonSerializer.Serialize(_configs);
        ApplicationData.Current.LocalSettings.Values[SettingsKey] = json;
    }

    public IEnumerable<HotkeyConfig> GetAllConfigs() => _configs;

    public HotkeyConfig? GetConfig(string actionId) => _configs.FirstOrDefault(c => c.ActionId == actionId);

    public void SaveConfig(string actionId, uint modifiers, uint virtualKey)
    {
        var config = GetConfig(actionId);
        if (config != null)
        {
            config.Modifiers = modifiers;
            config.VirtualKey = virtualKey;
            SaveToSettings();
            ConfigsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
