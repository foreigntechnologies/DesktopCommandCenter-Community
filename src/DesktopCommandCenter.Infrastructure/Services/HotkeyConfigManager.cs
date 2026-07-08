using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

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
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
        var filePath = Path.Combine(dir, "dcc_hotkeys.json");
        
        if (File.Exists(filePath))
        {
            try
            {
                var json = File.ReadAllText(filePath);
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
            // ── Community ──────────────────────────────────────────────────
            new HotkeyConfig { ActionId = "Dashboard",         DisplayName = "Dashboard",                Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "ProcessManager",    DisplayName = "Processos",                Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Temporizador",      DisplayName = "Temporizador",             Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Notes",             DisplayName = "Notas Rápidas",            Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "SystemUpdates",     DisplayName = "Update Center",            Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Clipboard",         DisplayName = "Smart Clipboard",          Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "ColorPicker",       DisplayName = "Color Picker",             Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "AlwaysOnTop",       DisplayName = "Always On Top",            Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Awake",             DisplayName = "Awake (Caffeine)",         Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Captura",           DisplayName = "Captura de Tela",          Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Tradutor",          DisplayName = "Tradutor",                 Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "CliCommands",       DisplayName = "Comandos CLI",             Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "AppsWorkspaces",    DisplayName = "Aplicativos",              Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "DeveloperHub",      DisplayName = "Developer Hub",            Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "FutureShell",       DisplayName = "FutureShell",              Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "ManageHotkeys",     DisplayName = "Gerenciar Atalhos",        Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Settings",          DisplayName = "Configurações",            Modifiers = 2, VirtualKey = 0xDE },
            // ── PRO (Enterprise) ──────────────────────────────────────────
            new HotkeyConfig { ActionId = "ChatFT",            DisplayName = "ChatFT",                   Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "PesquisaUniversal", DisplayName = "Pesquisa Universal",       Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Prompts",           DisplayName = "Biblioteca de Prompts",    Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Automacoes",        DisplayName = "Automações",               Modifiers = 0, VirtualKey = 0 },
            new HotkeyConfig { ActionId = "Marketplace",       DisplayName = "Marketplace e Plugins",    Modifiers = 0, VirtualKey = 0 },
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

        // Remove orphaned configs (e.g. removed features like Calculadora)
        var toRemove = _configs.Where(c => !defaults.Any(d => d.ActionId == c.ActionId)).ToList();
        foreach (var rm in toRemove)
        {
            _configs.Remove(rm);
            changed = true;
        }

        if (changed)
        {
            SaveToSettings();
        }
    }

    private void SaveToSettings()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
        Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, "dcc_hotkeys.json");
        var json = JsonSerializer.Serialize(_configs);
        File.WriteAllText(filePath, json);
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
