using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InverseProVisibility))]
    private bool _isProUnlocked = App.IsProUnlocked;

    public Visibility InverseProVisibility => IsProUnlocked ? Visibility.Collapsed : Visibility.Visible;

    public DashboardViewModel()
    {
        UpdateDateTime();

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => UpdateDateTime();
        _timer.Start();

        WeakReferenceMessenger.Default.Register<DesktopCommandCenter.UI.Messages.LicenseChangedMessage>(this, (r, m) =>
        {
            IsProUnlocked = m.Value;
        });
    }

    private void UpdateDateTime()
    {
        var timeFormat = App.GetTimeFormat();
        var dateFormat = App.GetDateFormat();

        CurrentTime = DateTime.Now.ToString(timeFormat);
        CurrentDate = DateTime.Now.ToString(dateFormat);
    }
}

