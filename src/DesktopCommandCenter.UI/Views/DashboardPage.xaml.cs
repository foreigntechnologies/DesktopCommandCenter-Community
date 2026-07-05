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
            var name = "Usuário";
            
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
                greeting = hour < 12 ? "Bom dia" : hour < 18 ? "Boa tarde" : "Boa noite";
            }
            
            if (DashGreeting != null)
            {
                DashGreeting.Text = $"{greeting}, {name}";
            }

            // Timeline Inteligente (Mock for Mission Control)
            var timelineItems = new System.Collections.ObjectModel.ObservableCollection<TimelineEvent>
            {
                new TimelineEvent { Time = "17:42", Icon = "\uE896", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = "Windows Update está Atualizado!" },
                new TimelineEvent { Time = "17:40", Icon = "\uE756", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = "FutureShell executou winget upgrade" },
                new TimelineEvent { Time = "17:31", Icon = "\uE7BA", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange), Message = "RAM chegou a 95%" },
                new TimelineEvent { Time = "17:20", Icon = "\uE702", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = "Bluetooth conectado" },
                new TimelineEvent { Time = "16:58", Icon = "\uEC19", IconColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen), Message = "ChatFT gerou um script PowerShell" }
            };
            if (TimelineList != null) TimelineList.ItemsSource = timelineItems;

            // Start real-time update timer
            _timer = new Microsoft.UI.Xaml.DispatcherTimer();
            _timer.Interval = System.TimeSpan.FromSeconds(1.5);
            _timer.Tick += Timer_Tick!;
            _timer.Start();
            
            // First tick manually to not wait 1.5s
            Timer_Tick(this, null!);

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

                string label = string.IsNullOrEmpty(d.VolumeLabel) ? (d.DriveType == System.IO.DriveType.Removable ? "Removível" : "Disco Local") : d.VolumeLabel;
                
                drivesList.Add(new DriveInfoModel
                {
                    Title = $"{label} {d.Name.TrimEnd('\\')}",
                    IconGlyph = icon,
                    PercentageUsed = pct,
                    UsedText = $"Usado: {usedGb:F1} GB",
                    FreeText = $"Livre: {freeGb:F1} GB",
                    TotalText = $"Total: {totalGb:F1} GB"
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
                t.Status = "Instalado";
                _devTools.Add(t);
            }
            if (_devTools.Count == 0)
            {
                _devTools.Add(new DevToolStatus { Name = "Nenhuma Ferramenta", Status = "Encontrada", ProcessName = "" });
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
                        HealthStatusText.Text = "Atenção: Uso de memória RAM elevado.";
                        
                        AIPromptTitle.Text = "Alerta de Desempenho";
                        AIPromptMessage.Text = $"Sua RAM atingiu {memStatus.dwMemoryLoad}%. Deseja que eu analise e encerre processos ociosos para liberar espaço?";
                        
                        if (AIPromptBtn1 != null) { AIPromptBtn1.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn1.Content = "Encontrar processos pesados"; }
                        if (AIPromptBtn2 != null) { AIPromptBtn2.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn2.Content = "Limpar memória em cache"; }
                        if (AIPromptBtn3 != null) { AIPromptBtn3.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed; }
                    }
                    else
                    {
                        HealthIcon.Glyph = "\uE73E"; // Checkmark
                        HealthIcon.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen);
                        HealthStatusText.Text = "Seu computador está saudável.";

                        AIPromptTitle.Text = "O que posso fazer por você?";
                        AIPromptMessage.Text = "Tudo parece tranquilo no momento. Posso ajudar com alguma automação ou pesquisa?";
                        
                        if (AIPromptBtn1 != null) { AIPromptBtn1.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn1.Content = "Criar nova Automação"; }
                        if (AIPromptBtn2 != null) { AIPromptBtn2.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn2.Content = "Abrir ChatFT"; }
                        if (AIPromptBtn3 != null) { AIPromptBtn3.Visibility = Microsoft.UI.Xaml.Visibility.Visible; AIPromptBtn3.Content = "Otimizar Rede (Flush DNS)"; }
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
            if (_devTools.Count > 0 && _devTools[0].Name != "Nenhuma Ferramenta")
            {
                var processes = System.Diagnostics.Process.GetProcesses().Select(p => p.ProcessName.ToLower()).ToHashSet();
                foreach (var tool in _devTools)
                {
                    if (processes.Contains(tool.ProcessName.ToLower()))
                    {
                        tool.Status = "Rodando";
                    }
                    else
                    {
                        tool.Status = "Instalado";
                    }
                }
            }
        }
        catch { }
    }

    private void AIPromptBtn1_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (AIPromptBtn1.Content?.ToString() == "Encontrar processos pesados")
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
        if (AIPromptBtn2.Content?.ToString() == "Limpar memória em cache")
        {
            try
            {
                AIPromptBtn2.IsEnabled = false;
                AIPromptBtn2.Content = "Limpando...";
                
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

                AIPromptBtn2.Content = "✓ Memória Otimizada!";
                await System.Threading.Tasks.Task.Delay(2000);
                AIPromptBtn2.Content = "Limpar memória em cache";
                AIPromptBtn2.IsEnabled = true;
            }
            catch
            {
                AIPromptBtn2.IsEnabled = true;
                AIPromptBtn2.Content = "Limpar memória em cache";
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
        if (AIPromptBtn3.Content?.ToString() == "Otimizar Rede (Flush DNS)")
        {
            try
            {
                AIPromptBtn3.IsEnabled = false;
                AIPromptBtn3.Content = "Limpando DNS...";
                
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

                AIPromptBtn3.Content = "✓ Rede Otimizada!";
                await System.Threading.Tasks.Task.Delay(2000);
                AIPromptBtn3.Content = "Otimizar Rede (Flush DNS)";
                AIPromptBtn3.IsEnabled = true;
            }
            catch
            {
                AIPromptBtn3.IsEnabled = true;
                AIPromptBtn3.Content = "Otimizar Rede (Flush DNS)";
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
        Status == "Rodando" ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen) :
        Status == "Instalado" ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DeepSkyBlue) :
        Status == "Atualizar" ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.IndianRed) :
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
