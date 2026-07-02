using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Globalization;
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

        // Refresh date/time immediately when the user switches the app language.
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateDateTime();

        WeakReferenceMessenger.Default.Register<DesktopCommandCenter.UI.Messages.LicenseChangedMessage>(this, (r, m) =>
        {
            IsProUnlocked = m.Value;
        });
    }

    private void UpdateDateTime()
    {
        var timeFormat = App.GetTimeFormat();
        var dateFormat = App.GetDateFormat();

        // Use the app-selected language culture, not the OS locale.
        // This ensures dates appear in English when the user picked English,
        // even if the OS is set to pt-BR or es-ES.
        CultureInfo culture;
        try
        {
            var langCode = App.GetAppLanguage(); // e.g. "en-US", "pt-BR", "es-ES"
            culture = new CultureInfo(langCode);
        }
        catch
        {
            culture = CultureInfo.CurrentCulture;
        }

        CurrentTime = DateTime.Now.ToString(timeFormat, culture);
        CurrentDate = DateTime.Now.ToString(dateFormat, culture);
    }
}

