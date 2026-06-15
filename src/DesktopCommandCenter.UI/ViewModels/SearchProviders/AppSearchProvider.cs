using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels.SearchProviders;

public class AppSearchProvider : ISearchProvider
{
    private readonly SearchResultItem[] _localApps = {
        new() { Title = "Calculadora", Description = "Acessar modo padrão, física ou química.", Type = "App", ActionPath = "Calculadora" },
        new() { Title = "Always On Top", Description = "Fixar janela atual acima das outras.", Type = "App", ActionPath = "AlwaysOnTop" },
        new() { Title = "Awake", Description = "Manter PC acordado.", Type = "App", ActionPath = "Awake" },
        new() { Title = "Color Picker", Description = "Copiar cores HEX/RGB da tela.", Type = "App", ActionPath = "ColorPicker" },
        new() { Title = "ChatFT", Description = "Chat com modelo Ollama.", Type = "App", ActionPath = "ChatFT" }
    };

    public Task<IEnumerable<SearchResultItem>> SearchAsync(string query, CancellationToken token)
    {
        var lowerQuery = query.ToLowerInvariant();
        var results = _localApps.Where(a => a.Title.ToLowerInvariant().Contains(lowerQuery) || a.Description.ToLowerInvariant().Contains(lowerQuery));
        return Task.FromResult(results);
    }
}
