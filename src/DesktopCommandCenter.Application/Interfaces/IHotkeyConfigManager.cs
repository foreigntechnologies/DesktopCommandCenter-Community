using System;
using System.Collections.Generic;

namespace DesktopCommandCenter.Application.Interfaces;

public class HotkeyConfig
{
    public string ActionId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public uint Modifiers { get; set; }
    public uint VirtualKey { get; set; }
}

public interface IHotkeyConfigManager
{
    IEnumerable<HotkeyConfig> GetAllConfigs();
    HotkeyConfig? GetConfig(string actionId);
    void SaveConfig(string actionId, uint modifiers, uint virtualKey);
    event EventHandler? ConfigsChanged;
}
