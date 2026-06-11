using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Microsoft.UI.Xaml;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string _currentTime = "";

    [ObservableProperty]
    private string _currentDate = "";

    [ObservableProperty]
    private string _welcomeMessage = "Bem-vindo de volta!";

    private readonly DispatcherTimer _timer;

    public DashboardViewModel()
    {
        UpdateDateTime();

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => UpdateDateTime();
        _timer.Start();
    }

    private void UpdateDateTime()
    {
        var timeFormat = App.GetTimeFormat();
        var dateFormat = App.GetDateFormat();

        CurrentTime = DateTime.Now.ToString(timeFormat);
        CurrentDate = DateTime.Now.ToString(dateFormat);
    }
}

