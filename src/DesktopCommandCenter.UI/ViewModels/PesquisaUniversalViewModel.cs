using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.UI.ViewModels.SearchProviders;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class SearchResultItem : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ActionPath { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // App, Setting, File, Math, Terminal

    public string IconGlyph => Type switch
    {
        "Setting" => "\xE713", // Settings gear
        "File" => "\xE8A5",    // Document
        "Math" => "\xE8EF",    // Calculator
        "Terminal" => "\xE756",// Command Prompt
        "App" => "\xE71D",     // App window
        _ => "\xE71E"          // Search
    };

    public string DisplayType => Type switch
    {
        "Setting" => Helpers.LocalizationHelper.Instance.GetString("Search_TypeSetting"),
        "File" => Helpers.LocalizationHelper.Instance.GetString("Search_TypeFile"),
        "Math" => Helpers.LocalizationHelper.Instance.GetString("Search_TypeMath"),
        "Terminal" => Helpers.LocalizationHelper.Instance.GetString("Search_TypeTerminal"),
        "App" => Helpers.LocalizationHelper.Instance.GetString("Search_TypeApp"),
        _ => Type
    };
}

public partial class PesquisaUniversalViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchQuery = "";

    public ObservableCollection<SearchResultItem> Results { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewVisibility))]
    private SearchResultItem? _selectedItem;

    public Microsoft.UI.Xaml.Visibility PreviewVisibility => SelectedItem != null ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;

    private readonly List<ISearchProvider> _providers;
    private CancellationTokenSource? _searchCts;

    public PesquisaUniversalViewModel()
    {
        _providers = new List<ISearchProvider>
        {
            new AppSearchProvider(),
            new SystemSettingsProvider(),
            new MathProvider(),
            new TerminalProvider(),
            new FastFileSearchProvider()
        };
    }

    async partial void OnSearchQueryChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        Results.Clear();
        if (string.IsNullOrWhiteSpace(value)) return;

        try
        {
            var tasks = _providers.Select(p => p.SearchAsync(value, token));
            var resultsArray = await Task.WhenAll(tasks);

            if (token.IsCancellationRequested) return;

            foreach (var providerResults in resultsArray)
            {
                foreach (var item in providerResults)
                {
                    Results.Add(item);
                }
            }
        }
        catch { }
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
            else if (item.Type == "Terminal")
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {item.ActionPath} & pause",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
        }
        catch { }
    }
}
