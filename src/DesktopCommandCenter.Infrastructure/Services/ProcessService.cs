using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Application.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Services;

public class ProcessService : IProcessService
{
    private class ProcessState
    {
        public TimeSpan LastTotalProcessorTime;
        public DateTime LastCheckTime;
    }

    private readonly Dictionary<int, ProcessState> _processStates = new();

    public async Task<IEnumerable<ProcessInfo>> GetProcessesAsync()
    {
        return await Task.Run(() =>
        {
            var result = new List<ProcessInfo>();
            var processes = Process.GetProcesses();
            var currentIds = new HashSet<int>();
            int processorCount = Environment.ProcessorCount;

            // Fast WMI bulk query to get all ParentProcessIds at once
            var parentMap = new Dictionary<int, int>();
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT ProcessId, ParentProcessId FROM Win32_Process");
                using var moCollection = searcher.Get();
                foreach (ManagementObject mo in moCollection)
                {
                    int pid = Convert.ToInt32(mo["ProcessId"]);
                    int ppid = Convert.ToInt32(mo["ParentProcessId"]);
                    parentMap[pid] = ppid;
                }
            }
            catch
            {
                // WMI might fail on some restricted setups, we just ignore parent/orphan detection in that case
            }

            foreach (var p in processes)
            {
                currentIds.Add(p.Id);
            }

            foreach (var p in processes)
            {
                double cpuUsage = 0;
                double ramUsage = 0;
                bool isOrphan = false;
                int parentId = 0;

                if (parentMap.TryGetValue(p.Id, out int mappedParentId))
                {
                    parentId = mappedParentId;
                    // It is an orphan if parentId > 0 AND parent process no longer exists (not in currentIds)
                    // Processes 0 (Idle) and 4 (System) don't count as orphans
                    if (parentId > 0 && p.Id > 4 && !currentIds.Contains(parentId))
                    {
                        isOrphan = true;
                    }
                }

                try
                {
                    ramUsage = p.WorkingSet64 / 1024.0 / 1024.0; // Convert bytes to MB
                    
                    if (!_processStates.TryGetValue(p.Id, out var state))
                    {
                        state = new ProcessState
                        {
                            LastTotalProcessorTime = p.TotalProcessorTime,
                            LastCheckTime = DateTime.UtcNow
                        };
                        _processStates[p.Id] = state;
                    }
                    else
                    {
                        var now = DateTime.UtcNow;
                        var currentTotal = p.TotalProcessorTime;
                        var cpuUsedMs = (currentTotal - state.LastTotalProcessorTime).TotalMilliseconds;
                        var totalMsPassed = (now - state.LastCheckTime).TotalMilliseconds;
                        
                        if (totalMsPassed > 0)
                        {
                            cpuUsage = (cpuUsedMs / (totalMsPassed * processorCount)) * 100.0;
                            // Cap at 100% just in case of rounding errors
                            if (cpuUsage > 100.0) cpuUsage = 100.0;
                            if (cpuUsage < 0.0) cpuUsage = 0.0;
                        }

                        state.LastTotalProcessorTime = currentTotal;
                        state.LastCheckTime = now;
                    }
                }
                catch
                {
                    // Catch Access Denied exceptions for system processes
                }

                result.Add(new ProcessInfo
                {
                    Id = p.Id,
                    Name = p.ProcessName,
                    CpuUsage = cpuUsage,
                    RamUsageMB = ramUsage,
                    DiskUsageKBps = 0, // Disk requires heavier Win32_PerfFormattedData query which hurts performance
                    ParentId = parentId,
                    IsOrphan = isOrphan
                });
            }

            // Cleanup state for exited processes
            var keysToRemove = _processStates.Keys.Where(k => !currentIds.Contains(k)).ToList();
            foreach (var k in keysToRemove)
            {
                _processStates.Remove(k);
            }

            // Return sorted by CPU descending, then by RAM
            return result.OrderByDescending(p => p.CpuUsage).ThenByDescending(p => p.RamUsageMB);
        });
    }

    public async Task KillProcessTreeAsync(int processId)
    {
        await Task.Run(() =>
        {
            KillProcessTreeRecursive(processId);
        });
    }

    private void KillProcessTreeRecursive(int processId)
    {
        try
        {
            // First find children using WMI
            var children = new List<int>();
            using (var searcher = new ManagementObjectSearcher($"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId={processId}"))
            using (var moCollection = searcher.Get())
            {
                foreach (ManagementObject mo in moCollection)
                {
                    children.Add(Convert.ToInt32(mo["ProcessId"]));
                }
            }

            // Recursively kill children
            foreach (var childId in children)
            {
                KillProcessTreeRecursive(childId);
            }

            // Finally, kill the parent
            var parent = Process.GetProcessById(processId);
            parent.Kill();
        }
        catch
        {
            // Process may already have exited or access denied
        }
    }
}
