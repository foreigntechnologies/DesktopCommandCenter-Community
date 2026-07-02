using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

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
        LoadAllCommands();
        ApplyFilters();
    }

    private void AddCmd(string title, string command, string description, string category)
    {
        _allCommands.Add(new CliCommandItem
        {
            Title = title,
            Command = command,
            Description = description,
            Category = category
        });
    }

    private void LoadAllCommands()
    {
        // --- Bash / Zsh (Linux & macOS) ---
        string catBash = "Bash / Zsh";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_0"), "pwd", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_0"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_1"), "ls", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_1"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_2"), "ls -la", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_2"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_3"), "cd ..", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_3"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_4"), "cd ~", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_4"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_5"), "mkdir pasta", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_5"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_6"), "rmdir pasta", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_6"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_7"), "rm arquivo", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_7"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_8"), "rm -rf pasta", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_8"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_9"), "touch arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_9"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_10"), "cat arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_10"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_11"), "less arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_11"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_12"), "head arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_12"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_13"), "tail arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_13"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_14"), "cp origem destino", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_14"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_15"), "mv origem destino", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_15"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_0"), "find . -name \"*.js\"", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_0"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_1"), "grep \"texto\" arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_1"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_2"), "grep -r \"texto\" .", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_2"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_16"), "which node", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_16"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_17"), "whereis node", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_17"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_18"), "chmod +x script.sh", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_18"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_19"), "chmod 755 script.sh", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_19"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_20"), "chown user arquivo", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_20"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_21"), "ps aux", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_21"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_22"), "top", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_22"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_23"), "htop", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_23"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_24"), "kill PID", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_24"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_25"), "kill -9 PID", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_25"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_26"), "df -h", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_26"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_27"), "du -sh *", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_27"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_28"), "curl URL", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_28"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_29"), "wget URL", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_29"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_30"), "zip -r arquivo.zip pasta", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_30"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_31"), "unzip arquivo.zip", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_31"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_32"), "tar -cvf arquivo.tar pasta", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_32"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_33"), "tar -xvf arquivo.tar", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_33"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_34"), "env", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_34"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_35"), "export VAR=value", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_35"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_36"), "history", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_36"), catBash);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_37"), "clear", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_37"), catBash);

        // --- Git ---
        string catGit = "Git";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_38"), "git init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_38"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_39"), "git clone URL", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_39"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_3"), "git config --global user.name \"Seu Nome\"", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_3"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_4"), "git config --global user.email \"seu@email.com\"", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_4"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_40"), "git status", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_40"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_41"), "git add .", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_41"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_42"), "git add arquivo", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_42"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_5"), "git commit -m \"mensagem\"", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_5"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_43"), "git push", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_43"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_44"), "git push origin main", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_44"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_45"), "git pull", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_45"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_46"), "git fetch", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_46"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_47"), "git branch", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_47"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_48"), "git checkout branch", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_48"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_49"), "git switch branch", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_49"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_50"), "git checkout -b feature", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_50"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_51"), "git switch -c feature", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_51"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_52"), "git merge branch", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_52"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_53"), "git rebase main", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_53"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_54"), "git stash", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_54"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_55"), "git stash pop", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_55"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_56"), "git reset --hard", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_56"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_57"), "git revert HASH", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_57"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_58"), "git log", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_58"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_59"), "git log --oneline", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_59"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_60"), "git diff", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_60"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_61"), "git tag", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_61"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_62"), "git tag v1.0.0.0", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_62"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_63"), "git remote -v", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_63"), catGit);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_64"), "git cherry-pick HASH", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_64"), catGit);

        // --- GitHub CLI (gh) ---
        string catGh = "GitHub CLI (gh)";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_65"), "gh auth login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_65"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_66"), "gh auth status", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_66"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_67"), "gh repo create", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_67"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_68"), "gh repo clone dono/projeto", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_68"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_69"), "gh pr create", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_69"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_70"), "gh pr list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_70"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_71"), "gh pr view", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_71"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_72"), "gh pr merge", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_72"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_73"), "gh issue create", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_73"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_74"), "gh issue list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_74"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_75"), "gh workflow list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_75"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_76"), "gh workflow run workflow.yml", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_76"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_77"), "gh release create v1.0.0.0", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_77"), catGh);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_78"), "gh release list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_78"), catGh);

        // --- GitLab CLI ---
        string catGlab = "GitLab CLI (glab)";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_79"), "glab auth login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_79"), catGlab);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_80"), "glab repo clone dono/projeto", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_80"), catGlab);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_81"), "glab repo create", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_81"), catGlab);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_82"), "glab mr create", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_82"), catGlab);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_83"), "glab mr list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_83"), catGlab);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_84"), "glab issue create", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_84"), catGlab);

        // --- NPM ---
        string catNpm = "NPM";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_85"), "npm init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_85"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_86"), "npm init -y", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_86"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_87"), "npm install", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_87"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_88"), "npm install pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_88"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_89"), "npm install -D pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_89"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_90"), "npm uninstall pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_90"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_91"), "npm update", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_91"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_92"), "npm outdated", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_92"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_93"), "npm audit", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_93"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_94"), "npm audit fix", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_94"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_95"), "npm run start", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_95"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_96"), "npm run build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_96"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_97"), "npm run test", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_97"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_98"), "npm cache clean --force", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_98"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_99"), "npm ls", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_99"), catNpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_100"), "npm publish", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_100"), catNpm);

        // --- PNPM ---
        string catPnpm = "PNPM";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_101"), "pnpm install", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_101"), catPnpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_102"), "pnpm add pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_102"), catPnpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_103"), "pnpm remove pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_103"), catPnpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_104"), "pnpm update", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_104"), catPnpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_105"), "pnpm run build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_105"), catPnpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_106"), "pnpm run test", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_106"), catPnpm);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_107"), "pnpm store prune", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_107"), catPnpm);

        // --- Yarn ---
        string catYarn = "Yarn";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_108"), "yarn", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_108"), catYarn);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_109"), "yarn add pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_109"), catYarn);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_110"), "yarn remove pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_110"), catYarn);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_111"), "yarn upgrade", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_111"), catYarn);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_112"), "yarn build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_112"), catYarn);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_113"), "yarn test", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_113"), catYarn);

        // --- Bun ---
        string catBun = "Bun";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_114"), "bun init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_114"), catBun);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_115"), "bun install", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_115"), catBun);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_116"), "bun add pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_116"), catBun);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_117"), "bun remove pacote", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_117"), catBun);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_118"), "bun update", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_118"), catBun);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_119"), "bun run script.ts", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_119"), catBun);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_120"), "bun test", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_120"), catBun);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_121"), "bun create react app-name", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_121"), catBun);

        // --- Node.js & NPX ---
        string catNode = "Node.js & NPX";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_122"), "node", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_122"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_123"), "node app.js", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_123"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_124"), "node --watch app.js", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_124"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_125"), "node --test", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_125"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_126"), "node -v", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_126"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_127"), "npm -v", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_127"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_128"), "npx create-next-app@latest", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_128"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_129"), "npx create-react-app app", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_129"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_130"), "npx prisma studio", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_130"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_131"), "npx eslint .", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_131"), catNode);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_132"), "npx prettier --write .", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_132"), catNode);

        // --- Angular CLI ---
        string catAngular = "Angular CLI";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_133"), "ng new projeto", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_133"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_134"), "ng serve", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_134"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_135"), "ng build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_135"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_136"), "ng test", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_136"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_137"), "ng lint", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_137"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_138"), "ng generate component home", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_138"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_139"), "ng g c home", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_139"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_140"), "ng generate service auth", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_140"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_141"), "ng generate guard auth", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_141"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_142"), "ng generate module admin", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_142"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_143"), "ng add @angular/pwa", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_143"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_144"), "ng update", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_144"), catAngular);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_145"), "ng version", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_145"), catAngular);

        // --- Outros Frameworks Web (Vue, Nuxt, Svelte) ---
        string catOtherWeb = "Vue, Nuxt & Svelte";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_146"), "npm create vue@latest", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_146"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_147"), "npm run dev", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_147"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_148"), "npm run build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_148"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_149"), "npm run preview", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_149"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_150"), "npx nuxi init app-name", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_150"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_151"), "npx nuxi dev", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_151"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_152"), "npx nuxi build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_152"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_153"), "npx nuxi generate", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_153"), catOtherWeb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_154"), "npm create svelte@latest app-name", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_154"), catOtherWeb);

        // --- Docker & Docker Compose ---
        string catDocker = "Docker & Compose";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_155"), "docker version", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_155"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_156"), "docker pull nginx", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_156"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_157"), "docker images", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_157"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_158"), "docker build -t app .", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_158"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_159"), "docker run -p 8080:80 app", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_159"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_160"), "docker ps", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_160"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_161"), "docker ps -a", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_161"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_162"), "docker stop CONTAINER", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_162"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_163"), "docker start CONTAINER", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_163"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_164"), "docker restart CONTAINER", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_164"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_165"), "docker rm CONTAINER", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_165"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_166"), "docker rmi IMAGE", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_166"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_167"), "docker logs CONTAINER", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_167"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_168"), "docker exec -it CONTAINER bash", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_168"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_169"), "docker system prune", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_169"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_170"), "docker compose up", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_170"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_171"), "docker compose up -d", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_171"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_172"), "docker compose down", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_172"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_173"), "docker compose build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_173"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_174"), "docker compose logs", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_174"), catDocker);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_175"), "docker compose ps", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_175"), catDocker);

        // --- Kubernetes, Minikube & OpenShift ---
        string catK8s = "Kubernetes & Orchestrators";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_176"), "kubectl version", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_176"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_177"), "kubectl get pods", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_177"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_178"), "kubectl get svc", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_178"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_179"), "kubectl get deployments", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_179"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_180"), "kubectl describe pod POD_NAME", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_180"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_181"), "kubectl logs POD_NAME", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_181"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_182"), "kubectl exec -it POD_NAME -- bash", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_182"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_183"), "kubectl apply -f deployment.yaml", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_183"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_184"), "kubectl delete -f deployment.yaml", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_184"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_185"), "kubectl scale deployment app --replicas=5", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_185"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_186"), "kubectl rollout restart deployment app", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_186"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_187"), "minikube start", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_187"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_188"), "minikube stop", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_188"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_189"), "minikube dashboard", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_189"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_190"), "minikube tunnel", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_190"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_191"), "oc login -u developer", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_191"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_192"), "oc project meu-projeto", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_192"), catK8s);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_193"), "oc projects", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_193"), catK8s);

        // --- Terraform, OpenTofu & Pulumi ---
        string catIaC = "Infraestrutura como Código";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_194"), "terraform init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_194"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_195"), "terraform plan", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_195"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_196"), "terraform apply", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_196"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_197"), "terraform destroy", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_197"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_198"), "terraform validate", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_198"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_199"), "terraform fmt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_199"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_200"), "tofu init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_200"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_201"), "tofu plan", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_201"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_202"), "tofu apply", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_202"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_203"), "pulumi login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_203"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_204"), "pulumi stack init dev", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_204"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_205"), "pulumi up", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_205"), catIaC);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_206"), "pulumi destroy", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_206"), catIaC);

        // --- Ansible & Vagrant ---
        string catAutomation = "Ansible & Vagrant";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_207"), "ansible all -m ping", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_207"), catAutomation);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_208"), "ansible-playbook -i hosts site.yml", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_208"), catAutomation);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_209"), "ansible-vault encrypt segredos.yml", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_209"), catAutomation);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_210"), "vagrant init ubuntu/focal64", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_210"), catAutomation);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_211"), "vagrant up", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_211"), catAutomation);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_212"), "vagrant ssh", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_212"), catAutomation);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_213"), "vagrant halt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_213"), catAutomation);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_214"), "vagrant destroy", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_214"), catAutomation);

        // --- Cloud Providers (AWS, GCP, Azure) ---
        string catCloud = "Provedores de Nuvem";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_215"), "aws configure", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_215"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_216"), "aws sts get-caller-identity", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_216"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_217"), "aws s3 ls", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_217"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_218"), "aws s3 cp arq.txt s3://bucket/", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_218"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_219"), "aws s3 sync local/ s3://bucket/", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_219"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_220"), "aws ec2 describe-instances", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_220"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_221"), "gcloud init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_221"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_222"), "gcloud auth login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_222"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_223"), "gcloud projects list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_223"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_224"), "gcloud run deploy", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_224"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_225"), "az login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_225"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_226"), "az group create --name RG --location eastus", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_226"), catCloud);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_227"), "az storage account list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_227"), catCloud);

        // --- PaaS & Serverless (Vercel, Netlify, Railway, Fly.io) ---
        string catPaas = "PaaS & Deploy Rápido";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_228"), "vercel login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_228"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_229"), "vercel", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_229"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_230"), "vercel --prod", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_230"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_231"), "vercel env add NOME_VAR", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_231"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_232"), "netlify login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_232"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_233"), "netlify deploy", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_233"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_234"), "netlify deploy --prod", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_234"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_235"), "railway login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_235"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_236"), "railway up", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_236"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_237"), "fly deploy", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_237"), catPaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_238"), "fly logs", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_238"), catPaas);

        // --- Backend BaaS (Firebase, Supabase) ---
        string catBaas = "Firebase & Supabase";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_239"), "firebase login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_239"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_240"), "firebase init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_240"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_241"), "firebase deploy", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_241"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_242"), "firebase emulators:start", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_242"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_243"), "supabase login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_243"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_244"), "supabase init", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_244"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_245"), "supabase start", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_245"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_246"), "supabase stop", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_246"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_247"), "supabase db pull", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_247"), catBaas);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_248"), "supabase migration new nome", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_248"), catBaas);

        // --- Databases (SQL & NoSQL) ---
        string catDb = "Banco de Dados";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_249"), "psql -U usuario -d banco", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_249"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_250"), "\\l", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_250"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_251"), "\\dt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_251"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_252"), "mysql -u root -p", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_252"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_253"), "SHOW DATABASES;", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_253"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_254"), "sqlcmd -S localhost -U SA -P 'Senha'", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_254"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_255"), "mongosh", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_255"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_256"), "show dbs", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_256"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_257"), "db.collection.find()", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_257"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_258"), "redis-cli", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_258"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_259"), "SET chave valor", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_259"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_260"), "GET chave", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_260"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_261"), "sqlite3 banco.db", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_261"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_262"), ".tables", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_262"), catDb);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_263"), "cqlsh", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_263"), catDb);

        // --- Java Backend (Maven, Gradle, Spring Boot, Quarkus) ---
        string catJava = "Java & Frameworks";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_264"), "mvn clean", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_264"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_265"), "mvn compile", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_265"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_266"), "mvn test", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_266"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_267"), "mvn package", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_267"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_268"), "mvn install", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_268"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_269"), "mvn spring-boot:run", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_269"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_270"), "gradle clean", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_270"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_271"), "gradle build", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_271"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_272"), "gradle bootRun", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_272"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_273"), "java -jar app.jar", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_273"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_274"), "quarkus create app meu-app", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_274"), catJava);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_275"), "quarkus dev", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_275"), catJava);

        // --- Outros Frameworks Backend (NestJS, FastAPI, Django, Flask, Laravel) ---
        string catBackends = "Outros Backends (Nest, Python, PHP)";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_276"), "nest new projeto", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_276"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_277"), "nest generate module modulo", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_277"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_278"), "npm run start:dev", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_278"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_279"), "uvicorn main:app --reload", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_279"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_280"), "fastapi dev main.py", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_280"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_281"), "django-admin startproject nome", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_281"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_282"), "python manage.py runserver", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_282"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_283"), "python manage.py migrate", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_283"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_284"), "python manage.py createsuperuser", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_284"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_285"), "flask run", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_285"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_286"), "php artisan serve", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_286"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_287"), "php artisan migrate", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_287"), catBackends);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_288"), "php artisan make:controller Nome", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_288"), catBackends);

        // --- Mobile (Flutter, Android, React Native, Expo, Ionic/Capacitor) ---
        string catMobile = "Mobile Development";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_289"), "flutter create meu_app", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_289"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_290"), "flutter run", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_290"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_291"), "flutter build apk", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_291"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_292"), "flutter doctor", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_292"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_293"), "flutter pub get", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_293"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_294"), "adb devices", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_294"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_295"), "adb install app.apk", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_295"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_296"), "emulator -avd NomeEmulador", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_296"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_297"), "npx react-native init App", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_297"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_298"), "npx react-native start", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_298"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_299"), "npx react-native run-android", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_299"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_300"), "npx expo start", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_300"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_301"), "ionic start", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_301"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_302"), "ionic serve", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_302"), catMobile);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_303"), "npx cap sync", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_303"), catMobile);

        // --- IA / LLMs (OpenAI, Ollama, Hugging Face, MLflow, W&B) ---
        string catAi = "IA & LLMs";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_304"), "openai api models.list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_304"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_305"), "ollama pull llama3", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_305"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_306"), "ollama run llama3", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_306"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_307"), "ollama list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_307"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_308"), "ollama ps", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_308"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_309"), "huggingface-cli login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_309"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_310"), "huggingface-cli download repo-id", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_310"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_311"), "mlflow ui", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_311"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_312"), "mlflow models serve -m modelo", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_312"), catAi);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_313"), "wandb login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_313"), catAi);

        // --- Observabilidade (Grafana, Prometheus, Datadog, New Relic) ---
        string catObs = "Observabilidade";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_314"), "grafana-cli plugins install id", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_314"), catObs);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_315"), "prometheus --config.file=config.yml", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_315"), catObs);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_316"), "datadog-agent status", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_316"), catObs);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_317"), "datadog-agent restart", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_317"), catObs);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_318"), "newrelic login", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_318"), catObs);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_319"), "newrelic entity list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_319"), catObs);

        // --- Windows Package Managers (Chocolatey) & PowerShell ---
        string catWin = "Windows (Choco & PowerShell)";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_320"), "choco install nodejs", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_320"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_321"), "choco uninstall nodejs", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_321"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_322"), "choco upgrade nodejs", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_322"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_323"), "choco upgrade all", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_323"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_324"), "choco search chrome", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_324"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_325"), "choco outdated", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_325"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_326"), "Get-Process | Sort-Object CPU -Descending | Select-Object -First 10", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_326"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_6"), "Restart-NetAdapter -Name \"Ethernet\"", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_6"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_327"), "Get-NetIPAddress -AddressFamily IPv4 | Select-Object IPAddress", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_327"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_328"), "Clear-DnsClientCache", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_328"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_7"), "Get-ChildItem -Path C:\\ -Filter \"*.pdf\" -Recurse -ErrorAction SilentlyContinue", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_7"), catWin);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_8"), "Stop-Process -Name \"chrome\" -Force", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_8"), catWin);

        // --- macOS Essentials (macOS Native Commands) ---
        string catMac = "macOS Essentials";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_Ext_9"), "/bin/bash -c \"$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\"", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_Ext_9"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_329"), "brew install git", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_329"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_330"), "brew update", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_330"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_331"), "brew upgrade", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_331"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_332"), "brew uninstall node", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_332"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_333"), "brew list", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_333"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_334"), "ping google.com", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_334"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_335"), "ifconfig", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_335"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_336"), "netstat -an", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_336"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_337"), "nano arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_337"), catMac);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_338"), "vim arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_338"), catMac);

        // --- Linux Essentials ---
        string catLinux = "Linux Essentials";
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_339"), "rsync -avz pasta/ destino/", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_339"), catLinux);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_340"), "scp arquivo.txt user@ip:/caminho/", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_340"), catLinux);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_341"), "tmux", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_341"), catLinux);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_342"), "jq .campo arquivo.json", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_342"), catLinux);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_343"), "yq .campo arquivo.yml", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_343"), catLinux);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_344"), "sed 's/antigo/novo/g' arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_344"), catLinux);
        AddCmd(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_T_345"), "awk '{print $1}' arquivo.txt", DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("CliCmd_D_345"), catLinux);

        // --- Categorias Iniciais ---
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

    [RelayCommand]
    private void CopyCommand(string commandText)
    {
        if (string.IsNullOrEmpty(commandText)) return;
        try
        {
            var package = new DataPackage();
            package.SetText(commandText);
            Clipboard.SetContent(package);
        }
        catch (Exception)
        {
            // Fail-safe
        }
    }
}
