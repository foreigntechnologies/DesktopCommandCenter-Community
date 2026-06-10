using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string _currentTime = DateTime.Now.ToString("HH:mm");

    [ObservableProperty]
    private string _currentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy");

    [ObservableProperty]
    private string _welcomeMessage = "Bem-vindo de volta!";
}

