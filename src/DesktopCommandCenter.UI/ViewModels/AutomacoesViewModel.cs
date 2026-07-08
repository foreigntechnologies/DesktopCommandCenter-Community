using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DesktopCommandCenter.UI.ViewModels;
public partial class AutomacaoRegra : ObservableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GatilhoCompleto))]
    private string _gatilho = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AcaoCompleta))]
    private string _acao = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GatilhoCompleto))]
    private string _gatilhoParametro = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AcaoCompleta))]
    private string _acaoParametro = string.Empty;

    [ObservableProperty]
    private string _acaoLinguagem = string.Empty;

    [ObservableProperty] private bool _isAtivo = true;

    public string GatilhoCompleto => string.IsNullOrEmpty(GatilhoParametro) ? Gatilho : $"{Gatilho} ({GatilhoParametro})";
    public string AcaoCompleta => string.IsNullOrEmpty(AcaoParametro) ? Acao : $"{Acao} ({AcaoParametro})";
}

public partial class AutomacoesViewModel : ObservableObject
{
    private readonly IAutomationEngine _automationEngine;
    private readonly string _filePath;

    public ObservableCollection<string> AvailableTriggers => new()
    {
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_1"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_2"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_3"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_4"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_5"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_6")
    };

    public ObservableCollection<string> AvailableActions => new()
    {
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_1"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_2"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_3"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_4"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_5"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_6"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_7"),
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_8")
    };

    public ObservableCollection<string> AvailableLanguages { get; } = new()
    {
        "PowerShell",
        "Bash",
        "Python",
        "JavaScript",
        "Java",
        "Golang"
    };

    public ObservableCollection<AutomacaoRegra> Regras { get; } = new();

    public AutomacoesViewModel(IAutomationEngine automationEngine)
    {
        _automationEngine = automationEngine;
        
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
        _filePath = Path.Combine(dir, "dcc_automations.json");
        
        LoadRules();
    }

    private void LoadRules()
    {
        Regras.Clear();
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<AutomacaoRegra>>(json);
                if (loaded != null)
                {
                    foreach (var rule in loaded)
                    {
                        rule.PropertyChanged += OnRegraPropertyChanged;
                        Regras.Add(rule);
                    }
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Erro ao carregar regras na ViewModel.");
        }

        // Fallback default
        var defaultRules = new[]
        {
            new AutomacaoRegra { Gatilho = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_1"), Acao = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_3"), IsAtivo = true },
            new AutomacaoRegra { Gatilho = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_2"), Acao = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_4"), IsAtivo = false },
            new AutomacaoRegra { Gatilho = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Trigger_3"), Acao = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_Action_8"), AcaoParametro = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auto_DefRule_ActionParam"), IsAtivo = true }
        };

        foreach (var rule in defaultRules)
        {
            rule.PropertyChanged += OnRegraPropertyChanged;
            Regras.Add(rule);
        }
        SaveRules();
    }

    public void SaveRules()
    {
        try
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var json = JsonSerializer.Serialize(Regras, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Erro ao salvar regras na ViewModel.");
        }
    }

    public void AddNovaRegra(string gatilho, string acao, string gatilhoParam, string acaoParam, string acaoLinguagem = "")
    {
        if (!string.IsNullOrEmpty(gatilho) && !string.IsNullOrEmpty(acao))
        {
            var nova = new AutomacaoRegra
            {
                Gatilho = gatilho,
                Acao = acao,
                GatilhoParametro = gatilhoParam ?? string.Empty,
                AcaoParametro = acaoParam ?? string.Empty,
                AcaoLinguagem = acaoLinguagem ?? string.Empty,
                IsAtivo = true
            };
            nova.PropertyChanged += OnRegraPropertyChanged;
            Regras.Add(nova);
            SaveRules();
            
            // Recarrega o motor de automação
            _automationEngine.ReloadRules();
        }
    }

    [RelayCommand]
    private void RemoveRegra(AutomacaoRegra regra)
    {
        if (regra != null)
        {
            regra.PropertyChanged -= OnRegraPropertyChanged;
            Regras.Remove(regra);
            SaveRules();
            
            // Recarrega o motor de automação
            _automationEngine.ReloadRules();
        }
    }

    private void OnRegraPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AutomacaoRegra.IsAtivo))
        {
            SaveRules();
            _automationEngine.ReloadRules();
        }
    }
}
