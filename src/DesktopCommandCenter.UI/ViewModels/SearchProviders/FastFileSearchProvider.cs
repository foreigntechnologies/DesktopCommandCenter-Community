using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels.SearchProviders;

public class FastFileSearchProvider : ISearchProvider
{
    public async Task<IEnumerable<SearchResultItem>> SearchAsync(string query, CancellationToken token)
    {
        var results = new List<SearchResultItem>();
        if (query.Length < 3 || query.StartsWith(">") || query.StartsWith("=")) 
            return results;

        await Task.Delay(200, token); // Small debounce

        var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var commonPrograms = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs";
        
        var dirsToSearch = new[] { 
            Path.Combine(userPath, "Desktop"),
            Path.Combine(userPath, "Documents"),
            Path.Combine(userPath, "Downloads"),
            Path.Combine(userPath, "Pictures"),
            Path.Combine(userPath, "Videos"),
            Path.Combine(userPath, @"AppData\Roaming\Microsoft\Windows\Start Menu\Programs"),
            commonPrograms
        };

        await Task.Run(() => {
            foreach (var fullPath in dirsToSearch)
            {
                if (token.IsCancellationRequested) break;
                if (Directory.Exists(fullPath))
                {
                    try {
                        var files = Directory.EnumerateFiles(fullPath, $"*{query}*", SearchOption.AllDirectories).Take(5);
                        foreach(var f in files)
                        {
                            if (token.IsCancellationRequested) break;
                            results.Add(new SearchResultItem {
                                Title = Path.GetFileNameWithoutExtension(f),
                                Description = f,
                                Type = "File",
                                ActionPath = f
                            });
                        }
                    } catch { } // Ignore permission errors
                }
            }
        }, token);

        return results;
    }
}
