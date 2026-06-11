using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class AutomacaoRegra : ObservableObject
{
    public string Gatilho { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public bool IsAtivo { get; set; } = true;
}

public partial class AutomacoesViewModel : ObservableObject
{
    public ObservableCollection<AutomacaoRegra> Regras { get; } = new()
    {
        new AutomacaoRegra { Gatilho = "Ao copiar um link do YouTube", Acao = "Extrair ID do vídeo" },
        new AutomacaoRegra { Gatilho = "Ao tirar um printscreen", Acao = "Salvar em C:\\Prints e extrair texto (OCR)", IsAtivo = false },
        new AutomacaoRegra { Gatilho = "Ao plugar Pendrive", Acao = "Rodar Antivírus no diretório raiz" }
    };

    [RelayCommand]
    private void NovaRegra()
    {
        Regras.Add(new AutomacaoRegra { Gatilho = "[Novo Gatilho]", Acao = "[Nova Ação]", IsAtivo = false });
    }
}
