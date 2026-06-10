using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopCommandCenter.UI.ViewModels;

public class CliCommandItem
{
    public string Title { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public partial class CliCommandsViewModel : ObservableObject
{
    private readonly ObservableCollection<CliCommandItem> _allCommands = new();
    
    public ObservableCollection<CliCommandItem> FilteredCommands { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "Todos";

    public CliCommandsViewModel()
    {
        LoadMockCommands();
        ApplyFilters();
    }

    private void LoadMockCommands()
    {
        _allCommands.Add(new CliCommandItem { Title = "Clear Screen", Command = "clear", Description = "Clears the terminal screen.", Category = "Bash" });
        _allCommands.Add(new CliCommandItem { Title = "Clear Screen", Command = "cls", Description = "Clears the terminal screen.", Category = "CMD" });
        _allCommands.Add(new CliCommandItem { Title = "List Files", Command = "ls -la", Description = "Lists all files including hidden ones with details.", Category = "Bash" });
        _allCommands.Add(new CliCommandItem { Title = "List Files", Command = "dir", Description = "Lists all files in current directory.", Category = "CMD" });
        _allCommands.Add(new CliCommandItem { Title = "Git Status", Command = "git status", Description = "Shows the working tree status.", Category = "Git" });
        _allCommands.Add(new CliCommandItem { Title = "NPM Install", Command = "npm install", Description = "Installs dependencies from package.json.", Category = "NPM" });
        _allCommands.Add(new CliCommandItem { Title = "Docker PS", Command = "docker ps", Description = "List running containers.", Category = "Docker" });

        Categories.Add("Todos");
        foreach (var category in _allCommands.Select(c => c.Category).Distinct().OrderBy(c => c))
        {
            Categories.Add(category);
        }
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        FilteredCommands.Clear();
        var query = SearchQuery?.ToLowerInvariant() ?? "";
        
        foreach (var cmd in _allCommands)
        {
            bool matchCategory = SelectedCategory == "Todos" || cmd.Category == SelectedCategory;
            bool matchQuery = string.IsNullOrEmpty(query) || 
                              cmd.Title.ToLowerInvariant().Contains(query) || 
                              cmd.Command.ToLowerInvariant().Contains(query) || 
                              cmd.Description.ToLowerInvariant().Contains(query);

            if (matchCategory && matchQuery)
            {
                FilteredCommands.Add(cmd);
            }
        }
    }
}
