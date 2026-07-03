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
    private string _welcomeMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dashboard_Welcome") ?? "Bem-vindo de volta!";

    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InverseProVisibility))]
    private bool _isProUnlocked = App.IsProUnlocked;

    public Visibility InverseProVisibility => IsProUnlocked ? Visibility.Collapsed : Visibility.Visible;

    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

    public DashboardViewModel()
    {
        // Capture the UI DispatcherQueue at construction time (must be on UI thread)
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        UpdateDateTime();

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => UpdateDateTime();
        _timer.Start();

        // Refresh date/time immediately when the user switches the app language.
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += OnLanguageChanged;

        WeakReferenceMessenger.Default.Register<DesktopCommandCenter.UI.Messages.LicenseChangedMessage>(this, (r, m) =>
        {
            // Ensure we update on the UI thread
            _dispatcherQueue?.TryEnqueue(() => IsProUnlocked = m.Value);
        });
    }

    private void OnLanguageChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_dispatcherQueue?.HasThreadAccess == true)
        {
            UpdateDateTime();
        }
        else
        {
            _dispatcherQueue?.TryEnqueue(UpdateDateTime);
        }
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
        WelcomeMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dashboard_Welcome") ?? "Bem-vindo de volta!";
    }
}

