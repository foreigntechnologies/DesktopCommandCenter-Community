using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using System;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class TemporizadorViewModel : ObservableObject
{
    private DispatcherQueueTimer _timer;
    private int _totalSeconds = 1500; // 25 min default

    [ObservableProperty]
    private string _timeDisplay = "25:00";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotRunning))]
    private bool _isRunning;

    public bool IsNotRunning => !IsRunning;

    [ObservableProperty]
    private int _inputMinutes = 25;

    public TemporizadorViewModel()
    {
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(DispatcherQueueTimer sender, object args)
    {
        if (_totalSeconds > 0)
        {
            _totalSeconds--;
            TimeSpan time = TimeSpan.FromSeconds(_totalSeconds);
            TimeDisplay = time.ToString(@"mm\:ss");
        }
        else
        {
            StopTimer();
            TimeDisplay = "00:00 (Tempo Esgotado!)";
        }
    }

    [RelayCommand]
    private void StartTimer()
    {
        if (!IsRunning)
        {
            if (_totalSeconds == 0) _totalSeconds = InputMinutes * 60;
            _timer.Start();
            IsRunning = true;
        }
    }

    [RelayCommand]
    private void StopTimer()
    {
        _timer.Stop();
        IsRunning = false;
    }

    [RelayCommand]
    private void ResetTimer()
    {
        StopTimer();
        _totalSeconds = InputMinutes * 60;
        TimeSpan time = TimeSpan.FromSeconds(_totalSeconds);
        TimeDisplay = time.ToString(@"mm\:ss");
    }
}
