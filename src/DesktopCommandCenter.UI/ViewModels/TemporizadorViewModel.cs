using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using System;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class TemporizadorViewModel : ObservableObject
{
    private DispatcherQueueTimer _timer;
    private DateTime _targetDate;

    [ObservableProperty]
    private string _timeDisplay = "00a 00m 00d 00:25:00.000";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotRunning))]
    private bool _isRunning;

    public bool IsNotRunning => !IsRunning;

    [ObservableProperty] private int _inputYears = 0;
    [ObservableProperty] private int _inputMonths = 0;
    [ObservableProperty] private int _inputDays = 0;
    [ObservableProperty] private int _inputHours = 0;
    [ObservableProperty] private int _inputMinutes = 25;
    [ObservableProperty] private int _inputSeconds = 0;
    [ObservableProperty] private int _inputMilliseconds = 0;

    public TemporizadorViewModel()
    {
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(30); // ~33fps for smooth milliseconds
        _timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(DispatcherQueueTimer sender, object args)
    {
        var remaining = _targetDate - DateTime.Now;
        if (remaining.TotalMilliseconds > 0)
        {
            UpdateDisplay(remaining);
        }
        else
        {
            StopTimer();
            TimeDisplay = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Temporizador_TimeUp");
        }
    }

    private void UpdateDisplay(TimeSpan remaining)
    {
        int years = (int)(remaining.TotalDays / 365);
        int remainingDaysForMonths = (int)(remaining.TotalDays % 365);
        int months = remainingDaysForMonths / 30;
        int days = remainingDaysForMonths % 30;

        TimeDisplay = $"{years:D2}a {months:D2}m {days:D2}d {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}.{remaining.Milliseconds:D3}";
    }

    [RelayCommand]
    private void StartTimer()
    {
        if (!IsRunning)
        {
            if (_targetDate == default || _targetDate <= DateTime.Now)
            {
                _targetDate = DateTime.Now
                    .AddYears(InputYears)
                    .AddMonths(InputMonths)
                    .AddDays(InputDays)
                    .AddHours(InputHours)
                    .AddMinutes(InputMinutes)
                    .AddSeconds(InputSeconds)
                    .AddMilliseconds(InputMilliseconds);
            }
            _timer.Start();
            IsRunning = true;
        }
    }

    [RelayCommand]
    private void StopTimer()
    {
        _timer.Stop();
        IsRunning = false;
        
        // When stopped, calculate the remaining time so we can resume later
        var remaining = _targetDate - DateTime.Now;
        if (remaining.TotalMilliseconds > 0)
        {
            // Re-calculate inputs based on remaining
            InputYears = (int)(remaining.TotalDays / 365);
            int remDays = (int)(remaining.TotalDays % 365);
            InputMonths = remDays / 30;
            InputDays = remDays % 30;
            InputHours = remaining.Hours;
            InputMinutes = remaining.Minutes;
            InputSeconds = remaining.Seconds;
            InputMilliseconds = remaining.Milliseconds;
        }
    }

    [RelayCommand]
    private void ResetTimer()
    {
        StopTimer();
        _targetDate = default;
        
        // Fallback to default format for display
        TimeDisplay = $"{InputYears:D2}a {InputMonths:D2}m {InputDays:D2}d {InputHours:D2}:{InputMinutes:D2}:{InputSeconds:D2}.{InputMilliseconds:D3}";
    }
}
