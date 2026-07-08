using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class DashboardPage : Page
{
    private Microsoft.UI.Xaml.DispatcherTimer? _timer;
    private ulong _prevIdleTime;
    private ulong _prevKernelTime;
    private ulong _prevUserTime;
    
    private long _prevNetworkBytes = 0;
    private DateTime _prevNetworkTime = DateTime.MinValue;

    private System.Collections.ObjectModel.ObservableCollection<DevToolStatus> _devTools = new();

    public DashboardPage()
    {
        this.InitializeComponent();
        this.Loaded += DashboardPage_Loaded;
        this.Unloaded += DashboardPage_Unloaded;
    }

    private void DashboardPage_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick!;
            _timer = null;
        }
    }

    private async void DashboardPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAuthService>((App.Current as App).Services);
            var user = await authService.GetCurrentUserAsync();
            var name = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DefaultUser") ?? "Usuário";
            
            if (user != null && !string.IsNullOrEmpty(user.DisplayName))
            {
                name = user.DisplayName;
            }
            else if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                name = user.Email.Split('@')[0];
            }

            var hour = System.DateTime.Now.Hour;
            var greetingKey = hour < 12 ? "GreetingMorning" : hour < 18 ? "GreetingAfternoon" : "GreetingEvening";
            var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
            
            var greeting = loc.GetString(greetingKey);
            if (string.IsNullOrEmpty(greeting) || greeting == greetingKey) 
            {
                greeting = hour < 12 
                    ? (DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Morning") ?? "Bom dia")
                    : hour < 18 
                        ? (DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Afternoon") ?? "Boa tarde")
                        : (DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Evening") ?? "Boa noite");
            }
            
            if (DashGreeting != null)
            {
                DashGreeting.Text = $"{greeting}, {name}";
            }

            // Timeline Inteligente (Mock for Mission Control)
            var timelineItems = new System.Collections.ObjectModel.ObservableCollection<TimelineEvent>
            {
                new TimelineEvent { Time = "17:42", Icon = "\uE896", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Timeline1")  },
                new TimelineEvent { Time = "17:40", Icon = "\uE756", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Timeline2")  },
                new TimelineEvent { Time = "17:31", Icon = "\uE7BA", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange), Message = string.Format(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Timeline3"), "95")  },
                new TimelineEvent { Time = "17:20", Icon = "\uE702", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Timeline4")  },
                new TimelineEvent { Time = "16:58", Icon = "\uEC19", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Timeline5")  }
            };
            if (TimelineList != null) TimelineList.ItemsSource = timelineItems;

            // Start real-time update timer
            _timer = new Microsoft.UI.Xaml.DispatcherTimer();
            _timer.Interval = System.TimeSpan.FromSeconds(1.5);
            _timer.Tick += Timer_Tick!;
            _timer.Start();
            
            // First tick manually to not wait 1.5s
            Timer_Tick(this, null!);

            if (StorageTitle != null) StorageTitle.Text = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_StorageTitle");
            if (SysShortcutsTitle != null) SysShortcutsTitle.Text = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_SysShortcuts");
            if (CardSettings != null) CardSettings.Title = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Settings");
            if (CardEnvVars != null) CardEnvVars.Title = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_EnvVars");
            if (CardRegEdit != null) CardRegEdit.Title = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_RegEdit");

            // Load Drives dynamically
            var drivesList = new System.Collections.ObjectModel.ObservableCollection<DriveInfoModel>();
            var drives = System.IO.DriveInfo.GetDrives().Where(d => d.IsReady);
            foreach (var d in drives)
            {
                var total = d.TotalSize;
                var free = d.TotalFreeSpace;
                var used = total - free;
                
                var totalGb = total / 1024.0 / 1024.0 / 1024.0;
                var freeGb = free / 1024.0 / 1024.0 / 1024.0;
                var usedGb = used / 1024.0 / 1024.0 / 1024.0;
                
                var pct = total > 0 ? (used / (double)total) * 100 : 0;
                
                string icon = d.DriveType == System.IO.DriveType.Removable ? "\uE88E" : // USB/Removable
                              d.DriveType == System.IO.DriveType.Network ? "\uE839" : // Network
                              "\uEDA2"; // Hard Drive/Default

                string removableTrans = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Removable") ?? "Removível";
                string localDiskTrans = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_LocalDisk") ?? "Disco Local";
                string label = string.IsNullOrEmpty(d.VolumeLabel) ? (d.DriveType == System.IO.DriveType.Removable ? removableTrans : localDiskTrans) : d.VolumeLabel;
                
                string usedTrans = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Used") ?? "Usado: {0} GB";
                string freeTrans = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Free") ?? "Livre: {0} GB";
                string totalTrans = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Total") ?? "Total: {0} GB";

                drivesList.Add(new DriveInfoModel
                {
                    Title = $"{label} {d.Name.TrimEnd('\\')}",
                    IconGlyph = icon,
                    PercentageUsed = pct,
                    UsedText = string.Format(usedTrans, usedGb.ToString("F1")),
                    FreeText = string.Format(freeTrans, freeGb.ToString("F1")),
                    TotalText = string.Format(totalTrans, totalGb.ToString("F1"))
                });
            }
            if (DrivesItemsControl != null)
            {
                DrivesItemsControl.ItemsSource = drivesList;
            }

            if (DevToolsList != null && DevToolsList.ItemsSource == null)
            {
                DevToolsList.ItemsSource = _devTools;
                _ = LoadDevToolsAsync();
            }

            _ = LoadRealCardsDataAsync();

            _prevNetworkTime = DateTime.Now;
            _prevNetworkBytes = GetTotalNetworkBytes();
        }
        catch { }
    }

    private long GetTotalNetworkBytes()
    {
        try
        {
            var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && 
                             ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback);
            long total = 0;
            foreach (var ni in interfaces)
            {
                var stats = ni.GetIPv4Statistics();
                total += stats.BytesReceived + stats.BytesSent;
            }
            return total;
        }
        catch { return 0; }
    }

    private async System.Threading.Tasks.Task LoadRealCardsDataAsync()
    {
        try
        {
            // FutureShell Quick Commands
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC", "dcc_quick_commands.json");
            if (System.IO.File.Exists(path))
            {
                var json = await System.IO.File.ReadAllTextAsync(path);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var cmds = doc.RootElement.EnumerateArray().Select(x => x.GetProperty("Command").GetString()).Where(x => !string.IsNullOrEmpty(x)).ToList();
                
                if (cmds.Count > 0 && FutureShellCmd1 != null) FutureShellCmd1.Text = cmds[0];
                if (cmds.Count > 1 && FutureShellCmd2 != null) FutureShellCmd2.Text = cmds[1];
                if (cmds.Count > 2 && FutureShellCmd3 != null) FutureShellCmd3.Text = cmds[2];
            }
            else
            {
                if (FutureShellCmd1 != null) FutureShellCmd1.Text = "Nenhum comando salvo.";
                if (FutureShellCmd2 != null) FutureShellCmd2.Text = "";
                if (FutureShellCmd3 != null) FutureShellCmd3.Text = "";
            }

            // ChatFT (No persistence implemented yet, using placeholder)
            if (ChatFT1 != null) ChatFT1.Text = "Nenhuma conversa encontrada.";
            if (ChatFT2 != null) ChatFT2.Text = "";
            if (ChatFT3 != null) ChatFT3.Text = "";
        }
        catch { }
    }

    private long GetTotalNetworkSpeed()
    {
        try
        {
            var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && 
                             ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback);
            long speed = 0;
            foreach (var ni in interfaces)
            {
                speed += ni.Speed;
            }
            return speed;
        }
        catch { return 0; }
    }

    private async System.Threading.Tasks.Task LoadDevToolsAsync()
    {
        var possibleTools = new System.Collections.Generic.List<DevToolStatus>
        {
            new DevToolStatus { Name = "Visual Studio", ProcessName = "devenv", Executable = "devenv.exe" },
            new DevToolStatus { Name = "VS Code", ProcessName = "code", Executable = "code.cmd" },
            new DevToolStatus { Name = "Docker", ProcessName = "Docker Desktop", Executable = "docker.exe" },
            new DevToolStatus { Name = "Node.js", ProcessName = "node", Executable = "node.exe" },
            new DevToolStatus { Name = "Python", ProcessName = "python", Executable = "python.exe" },
            new DevToolStatus { Name = "Java", ProcessName = "java", Executable = "java.exe" },
            new DevToolStatus { Name = "PostgreSQL", ProcessName = "postgres", Executable = "psql.exe" },
            new DevToolStatus { Name = "MySQL", ProcessName = "mysqld", Executable = "mysql.exe" },
            new DevToolStatus { Name = "SQL Server", ProcessName = "sqlservr", Executable = "sqlservr.exe" },
            new DevToolStatus { Name = "MongoDB", ProcessName = "mongod", Executable = "mongod.exe" },
            new DevToolStatus { Name = ".NET", ProcessName = "dotnet", Executable = "dotnet.exe" },
            new DevToolStatus { Name = "Git", ProcessName = "git", Executable = "git.exe" }
        };

        var installedTools = new System.Collections.Generic.List<DevToolStatus>();

        await System.Threading.Tasks.Task.Run(() =>
        {
            foreach (var t in possibleTools)
            {
                if (IsCommandAvailable(t.Executable))
                {
                    installedTools.Add(t);
                }
            }
        });

        DispatcherQueue.TryEnqueue(() => 
        {
            _devTools.Clear();
            foreach (var t in installedTools)
            {
                t.Status = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevInstalled");
                _devTools.Add(t);
            }
            if (_devTools.Count == 0)
            {
                _devTools.Add(new DevToolStatus { Name = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevNoTools"), Status = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevFound"), ProcessName = "" });
            }
        });
    }

    private bool IsCommandAvailable(string command)
    {
        try
        {
            var proc = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            return proc.ExitCode == 0;
        }
        catch { return false; }
    }

    private void Timer_Tick(object? sender, object? e)
    {
        try
        {
            // Update Date and Time
            var cultureCode = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.CurrentCulture;
            var culture = new System.Globalization.CultureInfo(cultureCode);
            
            if (DashDateText != null)
            {
                var dateFormat = App.GetDateFormat();
                if (string.IsNullOrEmpty(dateFormat)) dateFormat = "dddd, dd MMMM yyyy";
                DashDateText.Text = System.DateTime.Now.ToString(dateFormat, culture);
            }
            if (DashTimeText != null)
            {
                var timeFormat = App.GetTimeFormat();
                if (string.IsNullOrEmpty(timeFormat)) timeFormat = "HH:mm";
                DashTimeText.Text = System.DateTime.Now.ToString(timeFormat, culture);
            }

            // Update RAM
            var memStatus = new MEMORYSTATUSEX();
            memStatus.Init();
            if (NativeMethods.GlobalMemoryStatusEx(ref memStatus) && RamText != null)
            {
                var totalRamGb = memStatus.ullTotalPhys / 1024.0 / 1024.0 / 1024.0;
                var usedRamGb = (memStatus.ullTotalPhys - memStatus.ullAvailPhys) / 1024.0 / 1024.0 / 1024.0;
                RamText.Text = $"{memStatus.dwMemoryLoad}%"; // Only percentage for cleaner look

                // Intelligent AI Alert Logic
                if (HealthIcon != null && HealthStatusText != null && AIPromptTitle != null && AIPromptMessage != null)
                {
                    if (memStatus.dwMemoryLoad >= 90)
                    {
                        HealthIcon.Glyph = "\uE7BA"; // Warning
                        HealthIcon.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                        HealthStatusText.Text = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_HealthWarn");
                        
                        AIPromptTitle.Text = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_PerfAlert");
                        AIPromptMessage.Text = string.Format(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_PerfAlertDesc"), memStatus.dwMemoryLoad);
                        
                        if (AIPromptBtn1 != null) { AIPromptBtn1.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn1.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFindHeavy"); }
                        if (AIPromptBtn2 != null) { AIPromptBtn2.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn2.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearCache"); }
                        if (AIPromptBtn3 != null) { AIPromptBtn3.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed; }
                    }
                    else
                    {
                        HealthIcon.Glyph = "\uE73E"; // Checkmark
                        HealthIcon.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen);
                        HealthStatusText.Text = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_HealthOk");

                        AIPromptTitle.Text = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_AiAsk");
                        AIPromptMessage.Text = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_AiAskDesc");
                        
                        if (AIPromptBtn1 != null) { AIPromptBtn1.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn1.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnNewAuto"); }
                        if (AIPromptBtn2 != null) { AIPromptBtn2.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn2.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnOpenChatFT"); }
                        if (AIPromptBtn3 != null) { AIPromptBtn3.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn3.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFlushDns"); }
                    }
                }
            }

            // Update CPU
            if (NativeMethods.GetSystemTimes(out var idle, out var kernel, out var user) && CpuText != null)
            {
                ulong curIdle = idle.ToULong();
                ulong curKernel = kernel.ToULong();
                ulong curUser = user.ToULong();

                if (_prevIdleTime != 0)
                {
                    ulong sysDiff = (curKernel - _prevKernelTime) + (curUser - _prevUserTime);
                    ulong idleDiff = curIdle - _prevIdleTime;
                    
                    if (sysDiff > 0)
                    {
                        double cpuPct = (sysDiff - idleDiff) * 100.0 / sysDiff;
                        if (cpuPct < 0) cpuPct = 0;
                        if (cpuPct > 100) cpuPct = 100;
                        CpuText.Text = $"{cpuPct:F0}%";
                    }
                }
                
                _prevIdleTime = curIdle;
                _prevKernelTime = curKernel;
                _prevUserTime = curUser;
            }

            // Update Network
            long currentBytes = GetTotalNetworkBytes();
            DateTime currentTime = DateTime.Now;

            double seconds = (currentTime - _prevNetworkTime).TotalSeconds;
            if (seconds > 0 && NetworkText != null)
            {
                long bytesPerSec = (long)((currentBytes - _prevNetworkBytes) / seconds);
                long totalSpeed = GetTotalNetworkSpeed(); // bits per sec
                
                if (totalSpeed > 0)
                {
                    double utilization = (bytesPerSec * 8.0 / totalSpeed) * 100.0;
                    if (utilization > 100) utilization = 100;
                    if (utilization < 0) utilization = 0;
                    
                    // Mostra "<1%" se houver atividade mas for menos de 1%
                    if (utilization > 0 && utilization < 1)
                        NetworkText.Text = "<1%";
                    else
                        NetworkText.Text = $"{utilization:F0}%";
                }
            }
            _prevNetworkBytes = currentBytes;
            _prevNetworkTime = currentTime;

            // Update DevTools
            if (_devTools.Count > 0 && _devTools[0].Name != DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevNoTools"))
            {
                var processes = System.Diagnostics.Process.GetProcesses().Select(p => p.ProcessName.ToLower()).ToHashSet();
                foreach (var tool in _devTools)
                {
                    if (processes.Contains(tool.ProcessName.ToLower()))
                    {
                        tool.Status = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevRunning");
                    }
                    else
                    {
                        tool.Status = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevInstalled");
                    }
                }
            }
        }
        catch { }
    }

    private void AIPromptBtn1_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (AIPromptBtn1.Content?.ToString() == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFindHeavy"))
        {
            this.Frame?.Navigate(typeof(ProcessManagerPage));
        }
        else
        {
            this.Frame?.Navigate(typeof(AutomacoesPage));
        }
    }

    private async void AIPromptBtn2_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (AIPromptBtn2.Content?.ToString() == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearCache"))
        {
            try
            {
                AIPromptBtn2.IsEnabled = false;
                AIPromptBtn2.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearing");
                
                await System.Threading.Tasks.Task.Run(() =>
                {
                    foreach (var process in System.Diagnostics.Process.GetProcesses())
                    {
                        try
                        {
                            NativeMethods.EmptyWorkingSet(process.Handle);
                        }
                        catch { }
                    }
                });

                AIPromptBtn2.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnMemOptimized");
                await System.Threading.Tasks.Task.Delay(2000);
                AIPromptBtn2.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearCache");
                AIPromptBtn2.IsEnabled = true;
            }
            catch
            {
                AIPromptBtn2.IsEnabled = true;
                AIPromptBtn2.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearCache");
            }
        }
        else
        {
            var chatFt = new ChatFTWindow();
            chatFt.Activate();
        }
    }

    private async void AIPromptBtn3_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (AIPromptBtn3.Content?.ToString() == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFlushDns"))
        {
            try
            {
                AIPromptBtn3.IsEnabled = false;
                AIPromptBtn3.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearingDns");
                
                await System.Threading.Tasks.Task.Run(() =>
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ipconfig",
                        Arguments = "/flushdns",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    })?.WaitForExit();
                });

                AIPromptBtn3.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnNetOptimized");
                await System.Threading.Tasks.Task.Delay(2000);
                AIPromptBtn3.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFlushDns");
                AIPromptBtn3.IsEnabled = true;
            }
            catch
            {
                AIPromptBtn3.IsEnabled = true;
                AIPromptBtn3.Content = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFlushDns");
            }
        }
    }

    private void BtnOpenSettings_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo("ms-settings:")
            {
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(startInfo);
        }
        catch { }
    }

    private void BtnOpenEnvVars_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo("rundll32.exe")
            {
                Arguments = "sysdm.cpl,EditEnvironmentVariables",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(startInfo);
        }
        catch { }
    }

    private void BtnOpenRegEdit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo("regedit.exe")
            {
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(startInfo);
        }
        catch { }
    }
}

public class DevToolStatus : System.ComponentModel.INotifyPropertyChanged
{
    public string Name { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string Executable { get; set; } = string.Empty;
    
    private string _status = "Buscando...";
    public string Status 
    { 
        get => _status; 
        set 
        { 
            if (_status != value)
            {
                _status = value; 
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Status))); 
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(StatusColor))); 
            }
        } 
    }
    
    public Microsoft.UI.Xaml.Media.Brush StatusColor => 
        Status == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevRunning") ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen) :
        Status == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevInstalled") ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DeepSkyBlue) :
        Status == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevUpdate") ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.IndianRed) :
        new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DimGray);

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}

public class TimelineEvent
{
    public string Time { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Microsoft.UI.Xaml.Media.SolidColorBrush? IconColor { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DriveInfoModel
{
    public string Title { get; set; } = string.Empty;
    public string IconGlyph { get; set; } = string.Empty;
    public double PercentageUsed { get; set; }
    public string UsedText { get; set; } = string.Empty;
    public string FreeText { get; set; } = string.Empty;
    public string TotalText { get; set; } = string.Empty;
}

public static partial class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    public static extern bool GetSystemTimes(out FILETIME lpIdleTime, out FILETIME lpKernelTime, out FILETIME lpUserTime);

    [System.Runtime.InteropServices.DllImport("psapi.dll")]
    public static extern int EmptyWorkingSet(IntPtr hwProc);
}

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct MEMORYSTATUSEX
{
    public uint dwLength;
    public uint dwMemoryLoad;
    public ulong ullTotalPhys;
    public ulong ullAvailPhys;
    public ulong ullTotalPageFile;
    public ulong ullAvailPageFile;
    public ulong ullTotalVirtual;
    public ulong ullAvailVirtual;
    public ulong ullAvailExtendedVirtual;
    
    public void Init() { dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX)); }
}

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct FILETIME
{
    public uint dwLowDateTime;
    public uint dwHighDateTime;
    
    public ulong ToULong() => ((ulong)dwHighDateTime << 32) | dwLowDateTime;
}
