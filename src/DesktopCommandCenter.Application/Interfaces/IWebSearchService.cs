using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public class WebSearchResult
{
    public string Title { get; set; } = string.Empty;
    public string Snippet { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public interface IWebSearchService
{
    /// <summary>
    /// Searches the web for the given query and returns a list of result snippets.
    /// </summary>
    Task<List<WebSearchResult>> SearchAsync(string query, int maxResults = 5, CancellationToken cancellationToken = default);
}
