using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels.SearchProviders;

public class MathProvider : ISearchProvider
{
    public Task<IEnumerable<SearchResultItem>> SearchAsync(string query, CancellationToken token)
    {
        var results = new List<SearchResultItem>();
        query = query.Trim();

        // Check if query starts with = or contains math operators and numbers
        bool isMath = query.StartsWith("=") || 
                      (System.Text.RegularExpressions.Regex.IsMatch(query, @"^[\d\s\+\-\*\/\(\)\.]+$") && 
                       System.Text.RegularExpressions.Regex.IsMatch(query, @"[\+\-\*\/]"));

        if (isMath)
        {
            var expr = query.StartsWith("=") ? query.Substring(1) : query;
            if (string.IsNullOrWhiteSpace(expr)) return Task.FromResult((IEnumerable<SearchResultItem>)results);

            try
            {
                var dt = new DataTable();
                var result = dt.Compute(expr, "");
                
                if (result != null && result != DBNull.Value)
                {
                    results.Add(new SearchResultItem
                    {
                        Title = result.ToString() ?? "",
                        Description = $"Resultado de: {expr}",
                        Type = "Math",
                        ActionPath = result.ToString() ?? ""
                    });
                }
            }
            catch
            {
                // Invalid math expression, just ignore
            }
        }

        return Task.FromResult((IEnumerable<SearchResultItem>)results);
    }
}
