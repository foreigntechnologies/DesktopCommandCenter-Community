using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class AutomacaoRegra : ObservableObject
{
    [ObservableProperty] private string _gatilho = string.Empty;
    [ObservableProperty] private string _acao = string.Empty;
    [ObservableProperty] private bool _isAtivo = true;
}

public partial class AutomacoesViewModel : ObservableObject
{
    public ObservableCollection<string> AvailableTriggers { get; } = new()
    {
        "Ao copiar um link do YouTube",
        "Ao tirar um printscreen",
        "Ao plugar Pendrive",
        "Ao abrir o Visual Studio",
        "Ao ligar o computador",
        "A cada 1 hora"
    };

    public ObservableCollection<string> AvailableActions { get; } = new()
    {
        "Extrair ID do vídeo",
        "Salvar em C:\\Prints e extrair texto (OCR)",
        "Rodar Antivírus no diretório raiz",
        "Limpar área de transferência",
        "Ativar modo Não Perturbe",
        "Sincronizar arquivos para nuvem"
    };

    public ObservableCollection<AutomacaoRegra> Regras { get; } = new()
    {
        new AutomacaoRegra { Gatilho = "Ao copiar um link do YouTube", Acao = "Extrair ID do vídeo" },
        new AutomacaoRegra { Gatilho = "Ao tirar um printscreen", Acao = "Salvar em C:\\Prints e extrair texto (OCR)", IsAtivo = false },
        new AutomacaoRegra { Gatilho = "Ao plugar Pendrive", Acao = "Rodar Antivírus no diretório raiz" }
    };

    public void AddNovaRegra(string gatilho, string acao)
    {
        if (!string.IsNullOrEmpty(gatilho) && !string.IsNullOrEmpty(acao))
        {
            Regras.Add(new AutomacaoRegra { Gatilho = gatilho, Acao = acao, IsAtivo = true });
        }
    }

    [RelayCommand]
    private void RemoveRegra(AutomacaoRegra regra)
    {
        if (regra != null)
        {
            Regras.Remove(regra);
        }
    }
}
