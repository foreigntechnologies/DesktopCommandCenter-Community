using System;
using System.Linq;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Infrastructure.Services;
using Xunit;

namespace DesktopCommandCenter.UnitTests;

public class ProcessServiceTests
{
    private readonly IProcessService _processService;

    public ProcessServiceTests()
    {
        _processService = new ProcessService();
    }

    [Fact]
    public async Task GetProcessesAsync_ShouldReturnActiveProcesses()
    {
        // Act
        var processes = await _processService.GetProcessesAsync();

        // Assert
        Assert.NotNull(processes);
        Assert.NotEmpty(processes);
        
        // Ensure at least some fundamental system processes are visible (like our own test process)
        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
        var foundProcess = processes.FirstOrDefault(p => p.Id == currentProcess.Id);
        
        Assert.NotNull(foundProcess);
        Assert.Equal(currentProcess.ProcessName, foundProcess.Name);
    }

    [Fact]
    public async Task GetProcessesAsync_CpuAndRamUsageShouldBeCalculated()
    {
        // Arrange
        // Calling it once to initialize the state
        await _processService.GetProcessesAsync();
        
        // Wait a bit to accumulate CPU/RAM delta
        await Task.Delay(200);

        // Act
        var processes = await _processService.GetProcessesAsync();
        var currentProcess = processes.FirstOrDefault(p => p.Id == System.Diagnostics.Process.GetCurrentProcess().Id);

        // Assert
        Assert.NotNull(currentProcess);
        Assert.True(currentProcess.RamUsageMB > 0, "RAM usage should be greater than 0");
        Assert.True(currentProcess.CpuUsage >= 0, "CPU usage should be non-negative");
    }
}
