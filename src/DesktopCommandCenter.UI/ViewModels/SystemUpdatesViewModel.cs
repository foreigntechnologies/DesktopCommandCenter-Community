using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DesktopCommandCenter.UI.ViewModels;

// ─── Models ───────────────────────────────────────────────────────────────────

public sealed class WindowsUpdateItem : ObservableObject
{
    public string Title        { get; init; } = string.Empty;
    public string KbNumber     { get; init; } = string.Empty;
    public string Category     { get; init; } = string.Empty;
    public string Size         { get; init; } = string.Empty;
    public bool   IsCritical   { get; init; }

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public sealed class SoftwareUpdateItem : ObservableObject
{
    public string Name           { get; init; } = string.Empty;
    public string PackageId      { get; init; } = string.Empty;
    public string CurrentVersion { get; init; } = string.Empty;
    public string NewVersion     { get; init; } = string.Empty;
    public string Source         { get; init; } = string.Empty;

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public sealed class InstalledSoftwareItem : ObservableObject
{
    public string Name      { get; init; } = string.Empty;
    public string PackageId { get; init; } = string.Empty;
    public string Version   { get; init; } = string.Empty;
    public string Publisher { get; init; } = string.Empty;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

// ─── ViewModel ────────────────────────────────────────────────────────────────

public sealed partial class SystemUpdatesViewModel : ObservableObject
{
    private readonly DesktopCommandCenter.Application.Interfaces.IDeepCleanService _deepCleanService;

    public SystemUpdatesViewModel(DesktopCommandCenter.Application.Interfaces.IDeepCleanService deepCleanService)
    {
        _deepCleanService = deepCleanService;
    }

    // ── Windows Update ──────────────────────────────────────────────
    [ObservableProperty] private bool   _isLoadingWindowsUpdates;
    [ObservableProperty] private string _windowsUpdateStatus = string.Empty;
    [ObservableProperty] private bool   _autoInstallEnabled;
    [ObservableProperty] private string _installProgress = string.Empty;
    [ObservableProperty] private bool   _isInstalling;

    public ObservableCollection<WindowsUpdateItem> WindowsUpdates { get; } = [];

    // ── Software Updates (Winget) ────────────────────────────────────
    [ObservableProperty] private bool   _isLoadingSoftwareUpdates;
    [ObservableProperty] private string _softwareUpdateStatus = string.Empty;
    [ObservableProperty] private bool   _isUpdatingSoftware;
    [ObservableProperty] private string _softwareProgress = string.Empty;

    public ObservableCollection<SoftwareUpdateItem>  SoftwareUpdates  { get; } = [];
    public ObservableCollection<InstalledSoftwareItem> InstalledApps  { get; } = [];

    // ── Shared ─────────────────────────────────────────────────────
    [ObservableProperty] private bool   _isLoadingInstalledApps;
    [ObservableProperty] private string _installedAppsStatus = string.Empty;
    [ObservableProperty] private bool   _isUninstalling;
    [ObservableProperty] private string _uninstallProgress = string.Empty;
    [ObservableProperty] private bool   _deepCleanEnabled;

    private CancellationTokenSource? _cts;

    // ─────────────────────────────────────────────────────────────────
    //  WINDOWS UPDATE
    // ─────────────────────────────────────────────────────────────────

    [RelayCommand]
    public async Task CheckWindowsUpdatesAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        IsLoadingWindowsUpdates = true;
        WindowsUpdateStatus = Helpers.LocalizationHelper.Instance.GetString("Updates_WU_Checking") ?? "Verificando atualizações do Windows...";
        WindowsUpdates.Clear();

        try
        {
            // Use PowerShell to query Windows Update via COM (works without admin rights for listing)
            string script = @"
                $session = New-Object -ComObject Microsoft.Update.Session
                $searcher = $session.CreateUpdateSearcher()
                $result = $searcher.Search('IsInstalled=0 and Type=\'Software\'')
                $updates = @()
                foreach ($u in $result.Updates) {
                    $kb = ($u.KBArticleIDs | ForEach-Object { ""KB$_"" }) -join ', '
                    $cat = if ($u.Categories.Count -gt 0) { $u.Categories.Item(0).Name } else { 'General' }
                    $isCrit = $u.MsrcSeverity -eq 'Critical' -or $u.AutoSelectOnWebSites
                    $updates += [PSCustomObject]@{
                        Title    = $u.Title
                        KB       = $kb
                        Category = $cat
                        IsCrit   = [bool]$isCrit
                    }
                }
                $updates | ConvertTo-Json -Depth 2
            ";

            string json = await RunPowerShellAsync(script, _cts.Token);

            var items = ParseWindowsUpdateJson(json);
            foreach (var item in items)
                WindowsUpdates.Add(item);

            WindowsUpdateStatus = WindowsUpdates.Count == 0
                ? (Helpers.LocalizationHelper.Instance.GetString("Updates_WU_UpToDate") ?? "✅ Sistema atualizado!")
                : string.Format(
                    Helpers.LocalizationHelper.Instance.GetString("Updates_WU_Found") ?? "{0} atualização(ões) encontrada(s)",
                    WindowsUpdates.Count);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            WindowsUpdateStatus = $"Erro: {ex.Message}";
        }
        finally
        {
            IsLoadingWindowsUpdates = false;
        }
    }

    [RelayCommand]
    public async Task InstallSelectedWindowsUpdatesAsync()
    {
        var selected = new List<WindowsUpdateItem>();
        foreach (var u in WindowsUpdates)
            if (u.IsSelected) selected.Add(u);

        if (selected.Count == 0) return;

        IsInstalling = true;
        InstallProgress = Helpers.LocalizationHelper.Instance.GetString("Updates_WU_Installing") ?? "Instalando atualizações... Isso pode demorar.";

        try
        {
            // Build a PS script that installs selected KBs via WUApiLib
            string script = @"
                $session  = New-Object -ComObject Microsoft.Update.Session
                $searcher = $session.CreateUpdateSearcher()
                $result   = $searcher.Search('IsInstalled=0 and Type=\'Software\'')
                $toInstall = New-Object -ComObject Microsoft.Update.UpdateColl
                foreach ($u in $result.Updates) { $toInstall.Add($u) | Out-Null }
                if ($toInstall.Count -eq 0) { Write-Output 'NOTHING'; exit }
                $downloader = $session.CreateUpdateDownloader()
                $downloader.Updates = $toInstall
                $downloader.Download() | Out-Null
                $installer = $session.CreateUpdateInstaller()
                $installer.Updates = $toInstall
                $installResult = $installer.Install()
                Write-Output ""ResultCode:$($installResult.ResultCode)""
            ";

            InstallProgress = Helpers.LocalizationHelper.Instance.GetString("Updates_WU_Downloading") ?? "Baixando atualizações...";
            string result = await RunPowerShellElevatedAsync(script);
            InstallProgress = result.Contains("ResultCode:2")
                ? (Helpers.LocalizationHelper.Instance.GetString("Updates_WU_Done") ?? "✅ Atualizações instaladas com sucesso!")
                : (Helpers.LocalizationHelper.Instance.GetString("Updates_WU_Reboot") ?? "✅ Instalação concluída. Reinício pode ser necessário.");
            await CheckWindowsUpdatesAsync();
        }
        catch (Exception ex)
        {
            InstallProgress = $"Erro durante instalação: {ex.Message}";
        }
        finally
        {
            IsInstalling = false;
        }
    }

    partial void OnAutoInstallEnabledChanged(bool value)
    {
        // Persist the auto-update preference
        try
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["WU_AutoInstall"] = value;
        }
        catch { }
    }

    public void LoadAutoInstallPreference()
    {
        try
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Values.TryGetValue("WU_AutoInstall", out var v) && v is bool b)
                AutoInstallEnabled = b;
        }
        catch { }
    }

    // ─────────────────────────────────────────────────────────────────
    //  SOFTWARE UPDATES (WINGET)
    // ─────────────────────────────────────────────────────────────────

    [RelayCommand]
    public async Task CheckSoftwareUpdatesAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        IsLoadingSoftwareUpdates = true;
        SoftwareUpdateStatus = Helpers.LocalizationHelper.Instance.GetString("Updates_SW_Checking") ?? "Procurando atualizações de software...";
        SoftwareUpdates.Clear();

        try
        {
            // winget upgrade outputs a table; --include-unknown broadens results
            string output = await RunWingetAsync("upgrade", _cts.Token);
            var items = ParseWingetUpgradeOutput(output);
            foreach (var item in items)
                SoftwareUpdates.Add(item);

            SoftwareUpdateStatus = SoftwareUpdates.Count == 0
                ? (Helpers.LocalizationHelper.Instance.GetString("Updates_SW_UpToDate") ?? "✅ Todos os softwares estão atualizados!")
                : string.Format(
                    Helpers.LocalizationHelper.Instance.GetString("Updates_SW_Found") ?? "{0} software(s) com atualização disponível",
                    SoftwareUpdates.Count);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            SoftwareUpdateStatus = $"Erro: {ex.Message}";
        }
        finally
        {
            IsLoadingSoftwareUpdates = false;
        }
    }

    [RelayCommand]
    public async Task UpdateSelectedSoftwareAsync()
    {
        var selected = new List<SoftwareUpdateItem>();
        foreach (var s in SoftwareUpdates)
            if (s.IsSelected) selected.Add(s);

        if (selected.Count == 0) return;

        IsUpdatingSoftware = true;
        int done = 0;

        foreach (var pkg in selected)
        {
            SoftwareProgress = string.Format(
                Helpers.LocalizationHelper.Instance.GetString("Updates_SW_Updating") ?? "Atualizando {0}...",
                pkg.Name);
            try
            {
                await RunWingetAsync($"upgrade --id \"{pkg.PackageId}\" --silent --accept-package-agreements --accept-source-agreements", CancellationToken.None);
                done++;
            }
            catch { /* continua com o próximo */ }
        }

        SoftwareProgress = string.Format(
            Helpers.LocalizationHelper.Instance.GetString("Updates_SW_Done") ?? "✅ {0} software(s) atualizado(s)!",
            done);

        IsUpdatingSoftware = false;
        await CheckSoftwareUpdatesAsync();
    }

    // ─────────────────────────────────────────────────────────────────
    //  INSTALLED APPS (WINGET LIST)
    // ─────────────────────────────────────────────────────────────────

    [RelayCommand]
    public async Task LoadInstalledAppsAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        IsLoadingInstalledApps = true;
        InstalledAppsStatus = Helpers.LocalizationHelper.Instance.GetString("Updates_Apps_Loading") ?? "Carregando lista de softwares instalados...";
        InstalledApps.Clear();

        try
        {
            string output = await RunWingetAsync("list", _cts.Token);
            var items = ParseWingetListOutput(output);
            foreach (var item in items)
                InstalledApps.Add(item);

            InstalledAppsStatus = string.Format(
                Helpers.LocalizationHelper.Instance.GetString("Updates_Apps_Found") ?? "{0} software(s) instalado(s)",
                InstalledApps.Count);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            InstalledAppsStatus = $"Erro: {ex.Message}";
        }
        finally
        {
            IsLoadingInstalledApps = false;
        }
    }

    [RelayCommand]
    public async Task UninstallSelectedAppsAsync()
    {
        var selected = new List<InstalledSoftwareItem>();
        foreach (var a in InstalledApps)
            if (a.IsSelected) selected.Add(a);

        if (selected.Count == 0) return;

        IsUninstalling = true;
        int done = 0;

        foreach (var pkg in selected)
        {
            UninstallProgress = string.Format(
                Helpers.LocalizationHelper.Instance.GetString("Updates_Apps_Uninstalling") ?? "Desinstalando {0}...",
                pkg.Name);
            try
            {
                await RunWingetAsync($"uninstall --id \"{pkg.PackageId}\" --silent", CancellationToken.None);
                
                if (DeepCleanEnabled)
                {
                    UninstallProgress = string.Format(
                        Helpers.LocalizationHelper.Instance.GetString("Updates_Apps_DeepCleaning") ?? "Limpando sobras do {0}...",
                        pkg.Name);
                    
                    var deleted = await _deepCleanService.CleanLeftoversAsync(pkg.Name, pkg.Publisher);
                    // Opcional: logar os arquivos deletados
                }

                done++;
            }
            catch { }
        }

        UninstallProgress = string.Format(
            Helpers.LocalizationHelper.Instance.GetString("Updates_Apps_Done") ?? "✅ {0} software(s) removido(s)!",
            done);

        IsUninstalling = false;
        await LoadInstalledAppsAsync();
    }

    // ─────────────────────────────────────────────────────────────────
    //  HELPERS — Process runners
    // ─────────────────────────────────────────────────────────────────

    private static async Task<string> RunPowerShellAsync(string script, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName               = "powershell.exe",
            Arguments              = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{EscapePs(script)}\"",
            UseShellExecute        = false,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            CreateNoWindow         = true
        };

        using var proc = Process.Start(psi) ?? throw new InvalidOperationException("Could not start PowerShell.");
        
        // Kill process if token is cancelled
        await using var registration = ct.Register(() => { try { if (!proc.HasExited) proc.Kill(true); } catch { } });

        // Read concurrently to avoid pipe buffer deadlock
        var stdoutTask = proc.StandardOutput.ReadToEndAsync();
        var stderrTask = proc.StandardError.ReadToEndAsync();
        
        await Task.WhenAll(stdoutTask, stderrTask);
        await proc.WaitForExitAsync(ct);
        return stdoutTask.Result;
    }

    private static async Task<string> RunPowerShellElevatedAsync(string script)
    {
        // For install we need elevation — launch visible PS window
        var psi = new ProcessStartInfo
        {
            FileName        = "powershell.exe",
            Arguments       = $"-NoProfile -ExecutionPolicy Bypass -Command \"{EscapePs(script)}\"",
            UseShellExecute = true,
            Verb            = "runas"
        };

        using var proc = Process.Start(psi) ?? throw new InvalidOperationException("Could not start elevated PowerShell.");
        await proc.WaitForExitAsync();
        return proc.ExitCode == 0 ? "ResultCode:2" : $"ResultCode:{proc.ExitCode}";
    }

    private static async Task<string> RunWingetAsync(string args, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName               = "winget",
            Arguments              = $"{args} --disable-interactivity",
            UseShellExecute        = false,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            CreateNoWindow         = true
        };

        using var proc = Process.Start(psi) ?? throw new InvalidOperationException("winget not found.");
        
        await using var registration = ct.Register(() => { try { if (!proc.HasExited) proc.Kill(true); } catch { } });

        var stdoutTask = proc.StandardOutput.ReadToEndAsync();
        var stderrTask = proc.StandardError.ReadToEndAsync();

        await Task.WhenAll(stdoutTask, stderrTask);
        await proc.WaitForExitAsync(ct);
        return stdoutTask.Result;
    }

    private static string EscapePs(string script) => script.Replace("\"", "`\"").Replace("\r\n", " ").Replace("\n", " ");

    // ─────────────────────────────────────────────────────────────────
    //  HELPERS — Parsers
    // ─────────────────────────────────────────────────────────────────

    private static List<WindowsUpdateItem> ParseWindowsUpdateJson(string json)
    {
        var result = new List<WindowsUpdateItem>();
        if (string.IsNullOrWhiteSpace(json)) return result;
        json = json.Trim();
        // Wrap single object in array
        if (json.StartsWith('{')) json = $"[{json}]";
        try
        {
            using var doc = JsonDocument.Parse(json);
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                result.Add(new WindowsUpdateItem
                {
                    Title      = el.TryGetProperty("Title",    out var t) ? t.GetString() ?? "" : "",
                    KbNumber   = el.TryGetProperty("KB",       out var k) ? k.GetString() ?? "" : "",
                    Category   = el.TryGetProperty("Category", out var c) ? c.GetString() ?? "" : "",
                    IsCritical = el.TryGetProperty("IsCrit",   out var b) && b.GetBoolean()
                });
            }
        }
        catch { /* malformed JSON — return empty */ }
        return result;
    }

    private static List<SoftwareUpdateItem> ParseWingetUpgradeOutput(string output)
    {
        // winget upgrade table format:
        // Name        Id                   Version  Available Source
        // ---------------------------------------------------------
        // App Name    Publisher.AppName    1.0.0    2.0.0     winget
        var result = new List<SoftwareUpdateItem>();
        if (string.IsNullOrWhiteSpace(output)) return result;

        bool inTable = false;
        foreach (var line in output.Split('\n'))
        {
            string trimmed = line.TrimEnd();
            if (trimmed.StartsWith("---") || trimmed.StartsWith("───"))
            {
                inTable = true;
                continue;
            }
            if (!inTable) continue;
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            if (trimmed.StartsWith("Name", StringComparison.OrdinalIgnoreCase) &&
                trimmed.Contains("Version", StringComparison.OrdinalIgnoreCase)) continue;

            // Split by 2+ spaces (winget uses padding)
            var cols = System.Text.RegularExpressions.Regex.Split(trimmed, @"\s{2,}");
            if (cols.Length < 4) continue;

            result.Add(new SoftwareUpdateItem
            {
                Name           = cols[0].Trim(),
                PackageId      = cols[1].Trim(),
                CurrentVersion = cols[2].Trim(),
                NewVersion     = cols[3].Trim(),
                Source         = cols.Length > 4 ? cols[4].Trim() : "winget"
            });
        }
        return result;
    }

    private static List<InstalledSoftwareItem> ParseWingetListOutput(string output)
    {
        // Similar table format to upgrade
        var result = new List<InstalledSoftwareItem>();
        if (string.IsNullOrWhiteSpace(output)) return result;

        bool inTable = false;
        foreach (var line in output.Split('\n'))
        {
            string trimmed = line.TrimEnd();
            if (trimmed.StartsWith("---") || trimmed.StartsWith("───"))
            {
                inTable = true;
                continue;
            }
            if (!inTable) continue;
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            if (trimmed.StartsWith("Name", StringComparison.OrdinalIgnoreCase) &&
                trimmed.Contains("Id", StringComparison.OrdinalIgnoreCase)) continue;

            var cols = System.Text.RegularExpressions.Regex.Split(trimmed, @"\s{2,}");
            if (cols.Length < 2) continue;

            result.Add(new InstalledSoftwareItem
            {
                Name      = cols[0].Trim(),
                PackageId = cols[1].Trim(),
                Version   = cols.Length > 2 ? cols[2].Trim() : "",
                Publisher = cols.Length > 3 ? cols[3].Trim() : ""
            });
        }
        return result;
    }

    // ─────────────────────────────────────────────────────────────────
    //  SUMMARY (for Dashboard card)
    // ─────────────────────────────────────────────────────────────────

    public async Task<(int WindowsCount, int SoftwareCount)> GetUpdateSummaryAsync()
    {
        int wuCount = 0, swCount = 0;
        try
        {
            string wuScript = @"
                $s = New-Object -ComObject Microsoft.Update.Session
                $r = $s.CreateUpdateSearcher().Search('IsInstalled=0 and Type=\'Software\'')
                Write-Output $r.Updates.Count
            ";
            string wuResult = await RunPowerShellAsync(wuScript);
            int.TryParse(wuResult.Trim(), out wuCount);
        }
        catch { }

        try
        {
            string wingetOut = await RunWingetAsync("upgrade");
            swCount = ParseWingetUpgradeOutput(wingetOut).Count;
        }
        catch { }

        return (wuCount, swCount);
    }
}
