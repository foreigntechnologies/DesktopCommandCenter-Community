using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class SearchResultItem : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ActionPath { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // App, Setting, File
}

public partial class PesquisaUniversalViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchQuery = "";

    public ObservableCollection<SearchResultItem> Results { get; } = new();

    private readonly SearchResultItem[] _allPossibleResults = {
        new() { Title = "Calculadora", Description = "Acessar modo padrão, física ou química.", Type = "App", ActionPath = "Calculadora" },
        new() { Title = "Always On Top", Description = "Fixar janela atual acima das outras.", Type = "App", ActionPath = "AlwaysOnTop" },
        new() { Title = "Awake", Description = "Manter PC acordado.", Type = "App", ActionPath = "Awake" },
        new() { Title = "Color Picker", Description = "Copiar cores HEX/RGB da tela.", Type = "App", ActionPath = "ColorPicker" },
        new() { Title = "ChatFT", Description = "Chat com modelo Ollama.", Type = "App", ActionPath = "ChatFT" },
        new() { Title = "Wi-Fi", Description = "Configurações de rede sem fio", Type = "Setting", ActionPath = "ms-settings:network-wifi" },
        new() { Title = "Bluetooth", Description = "Gerenciar dispositivos Bluetooth", Type = "Setting", ActionPath = "ms-settings:bluetooth" },
        new() { Title = "Tela / Monitor", Description = "Configurações de vídeo e tela", Type = "Setting", ActionPath = "ms-settings:display" },
        new() { Title = "Windows Update", Description = "Verificar atualizações do sistema", Type = "Setting", ActionPath = "ms-settings:windowsupdate" },
        new() { Title = "Aplicativos", Description = "Desinstalar ou gerenciar programas", Type = "Setting", ActionPath = "ms-settings:appsfeatures" }
    };

    private System.Threading.CancellationTokenSource? _searchCts;

    async partial void OnSearchQueryChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts = new System.Threading.CancellationTokenSource();
        var token = _searchCts.Token;

        Results.Clear();
        if (string.IsNullOrWhiteSpace(value)) return;

        var lowerQuery = value.ToLowerInvariant();
        
        // 1. Busca Local (Apps e Configurações)
        var matches = _allPossibleResults.Where(r => r.Title.ToLowerInvariant().Contains(lowerQuery) || r.Description.ToLowerInvariant().Contains(lowerQuery));
        foreach (var match in matches)
        {
            Results.Add(match);
        }

        // 2. Busca de Arquivos (Desktop, Documents, Downloads)
        if (value.Length >= 3)
        {
            try
            {
                await System.Threading.Tasks.Task.Delay(300, token); // Debounce
                
                await System.Threading.Tasks.Task.Run(() => {
                    var userPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                    var dirsToSearch = new[] { "Desktop", "Documents", "Downloads" };
                    
                    foreach (var d in dirsToSearch)
                    {
                        if (token.IsCancellationRequested) break;
                        var fullPath = System.IO.Path.Combine(userPath, d);
                        if (System.IO.Directory.Exists(fullPath))
                        {
                            var files = System.IO.Directory.EnumerateFiles(fullPath, $"*{value}*", System.IO.SearchOption.AllDirectories).Take(5);
                            foreach(var f in files)
                            {
                                if (token.IsCancellationRequested) break;
                                Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(() => {
                                    Results.Add(new SearchResultItem {
                                        Title = System.IO.Path.GetFileName(f),
                                        Description = f,
                                        Type = "File",
                                        ActionPath = f
                                    });
                                });
                            }
                        }
                    }
                }, token);
            }
            catch { }
        }
    }

    public void ExecuteItem(SearchResultItem item)
    {
        try
        {
            if (item.Type == "Setting" || item.Type == "File")
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = item.ActionPath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            else if (item.Type == "App")
            {
                // To be handled by the Page/NavigationService
            }
        }
        catch { }
    }
}
