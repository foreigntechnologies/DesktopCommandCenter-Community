using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels.SearchProviders;

public class TerminalProvider : ISearchProvider
{
    public Task<IEnumerable<SearchResultItem>> SearchAsync(string query, CancellationToken token)
    {
        var results = new List<SearchResultItem>();
        query = query.Trim();

        if (query.StartsWith(">") && query.Length > 1)
        {
            var command = query.Substring(1).Trim();
            if (!string.IsNullOrWhiteSpace(command))
            {
                results.Add(new SearchResultItem
                {
                    Title = $"Executar Comando: {command}",
                    Description = "Pressione Enter para executar no terminal",
                    Type = "Terminal",
                    ActionPath = command
                });
            }
        }

        return Task.FromResult((IEnumerable<SearchResultItem>)results);
    }
}
