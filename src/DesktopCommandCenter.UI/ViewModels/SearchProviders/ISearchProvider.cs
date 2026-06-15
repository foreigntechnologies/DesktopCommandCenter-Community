using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels.SearchProviders;

public interface ISearchProvider
{
    Task<IEnumerable<SearchResultItem>> SearchAsync(string query, CancellationToken token);
}
