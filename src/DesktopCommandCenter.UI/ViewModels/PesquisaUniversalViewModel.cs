using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class SearchResultItem : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public partial class PesquisaUniversalViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchQuery = "";

    public ObservableCollection<SearchResultItem> Results { get; } = new();

    private readonly SearchResultItem[] _allPossibleResults = {
        new() { Title = "Calculadora", Description = "Acessar modo padrão, física ou química.", Icon = "Calculator" },
        new() { Title = "Always On Top", Description = "Fixar janela atual acima das outras.", Icon = "DockRight" },
        new() { Title = "Awake", Description = "Manter PC acordado.", Icon = "Street" },
        new() { Title = "Color Picker", Description = "Copiar cores HEX/RGB da tela.", Icon = "Highlight" },
        new() { Title = "IA Local", Description = "Chat com modelo Ollama.", Icon = "Message" }
    };

    partial void OnSearchQueryChanged(string value)
    {
        Results.Clear();
        if (string.IsNullOrWhiteSpace(value)) return;

        var lowerQuery = value.ToLowerInvariant();
        var matches = _allPossibleResults.Where(r => r.Title.ToLowerInvariant().Contains(lowerQuery) || r.Description.ToLowerInvariant().Contains(lowerQuery));

        foreach (var match in matches)
        {
            Results.Add(match);
        }
    }
}
