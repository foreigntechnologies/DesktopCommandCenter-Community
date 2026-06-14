using System.Collections.Generic;

namespace DesktopCommandCenter.Application.Models;

public class ProcessInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public double RamUsageMB { get; set; }
    public double DiskUsageKBps { get; set; }
    public int ParentId { get; set; }
    public bool IsOrphan { get; set; }
    public List<double> CpuHistory { get; set; } = new();
    
    // Derived property for UI binding
    public string StatusText => IsOrphan ? "Órfão" : "Normal";
    public string CpuUsageText => CpuUsage.ToString("0.00");
    public string RamUsageText => RamUsageMB.ToString("0.00");
}
