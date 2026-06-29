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

    public ObservableCollection<string> AvailableTriggers { get; } = new()
    {
        "Ao copiar um link do YouTube",
        "Ao tirar um printscreen",
        "Ao plugar Pendrive",
        "A cada X minutos/horas",
        "Ao ligar o computador",
        "Ao abrir um aplicativo específico"
    };

    public ObservableCollection<string> AvailableActions { get; } = new()
    {
        "Abrir programa",
        "Executar script personalizado",
        "Extrair ID do vídeo",
        "Extrair texto de imagem via OCR",
        "Falar texto (Text-to-Speech)",
        "Executar script PowerShell ou CMD",
        "Limpar Área de Transferência",
        "Exibir notificação do sistema (Toast)"
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
            new AutomacaoRegra { Gatilho = "Ao copiar um link do YouTube", Acao = "Extrair ID do vídeo", IsAtivo = true },
            new AutomacaoRegra { Gatilho = "Ao tirar um printscreen", Acao = "Extrair texto de imagem via OCR", IsAtivo = false },
            new AutomacaoRegra { Gatilho = "Ao plugar Pendrive", Acao = "Exibir notificação do sistema (Toast)", AcaoParametro = "Novo dispositivo USB conectado!", IsAtivo = true }
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
