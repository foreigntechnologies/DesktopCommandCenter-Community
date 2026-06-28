using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Application.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class ProcessManagerViewModel : ObservableObject
{
    private readonly IProcessService _processService;
    private DispatcherQueueTimer? _pollingTimer;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public ObservableCollection<ProcessInfo> Processes { get; } = new();
    private ObservableCollection<ProcessInfo> _allProcesses = new();

    public ProcessManagerViewModel(IProcessService processService)
    {
        _processService = processService;
    }

    public void StartPolling()
    {
        if (_pollingTimer == null)
        {
            var dispatcher = DispatcherQueue.GetForCurrentThread();
            _pollingTimer = dispatcher.CreateTimer();
            _pollingTimer.Interval = TimeSpan.FromSeconds(2);
            _pollingTimer.Tick += async (s, e) => await RefreshProcessesAsync();
        }
        
        IsLoading = true;
        _ = RefreshProcessesAsync();
        _pollingTimer.Start();
    }

    public void StopPolling()
    {
        _pollingTimer?.Stop();
    }

    [RelayCommand]
    public async Task RefreshProcessesAsync()
    {
        try
        {
            var result = await _processService.GetProcessesAsync();
            
            // Avoid complete list wipe if possible for UI Virtualization performance
            _allProcesses.Clear();
            foreach (var p in result)
            {
                _allProcesses.Add(p);
            }

            ApplyFilter();
        }
        catch
        {
            // Ignore temporary access errors
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchQueryChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var dispatcher = DispatcherQueue.GetForCurrentThread();
        if (dispatcher == null) return;

        dispatcher.TryEnqueue(() => 
        {
            Processes.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchQuery) 
                ? _allProcesses 
                : _allProcesses.Where(p => p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            foreach (var p in filtered)
            {
                Processes.Add(p);
            }
        });
    }

    [RelayCommand]
    public async Task KillProcessTreeAsync(int processId)
    {
        try
        {
            await _processService.KillProcessTreeAsync(processId);
            await RefreshProcessesAsync();
        }
        catch
        {
            // Could log failure to kill
        }
    }
}
