using Microsoft.UI.Xaml.Controls;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class DeveloperHubPage : Page
{
    public System.Collections.ObjectModel.ObservableCollection<LocalAIStatus> LocalAIs { get; } = new();
    private Microsoft.UI.Xaml.DispatcherTimer? _timer;

    public DeveloperHubPage()
    {
        this.InitializeComponent();
        this.Loaded += DeveloperHubPage_Loaded;
        this.Unloaded += DeveloperHubPage_Unloaded;
    }

    private void DeveloperHubPage_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick!;
            _timer = null;
        }
    }

    private void DeveloperHubPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Initialize AI list
        if (LocalAIs.Count == 0)
        {
            LocalAIs.Add(new LocalAIStatus { Name = "Ollama", Endpoint = "http://localhost:11434" });
            LocalAIs.Add(new LocalAIStatus { Name = "LM Studio", Endpoint = "http://localhost:1234" });
        }

        CheckAIEndpointsAsync();

        _timer = new Microsoft.UI.Xaml.DispatcherTimer();
        _timer.Interval = System.TimeSpan.FromSeconds(5);
        _timer.Tick += Timer_Tick!;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, object? e)
    {
        CheckAIEndpointsAsync();
    }

    private async void CheckAIEndpointsAsync()
    {
        using var client = new System.Net.Http.HttpClient();
        client.Timeout = System.TimeSpan.FromSeconds(2);

        foreach (var ai in LocalAIs)
        {
            try
            {
                string testUrl = ai.Name == "Ollama" ? $"{ai.Endpoint}/" : $"{ai.Endpoint}/v1/models";
                var response = await client.GetAsync(testUrl);
                if (response.IsSuccessStatusCode)
                {
                    ai.StatusText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("DevHub_Running") ?? "Rodando";
                }
                else
                {
                    ai.StatusText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("DevHub_Error") ?? "Erro";
                }
            }
            catch
            {
                ai.StatusText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("DevHub_Offline") ?? "Offline";
            }
        }
    }
}

public class LocalAIStatus : System.ComponentModel.INotifyPropertyChanged
{
    public string Name { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;

    private string _statusText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("DevHub_Checking") ?? "Verificando...";
    public string StatusText
    {
        get => _statusText;
        set
        {
            if (_statusText != value)
            {
                _statusText = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(StatusText)));
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(StatusColor)));
            }
        }
    }

    public Microsoft.UI.Xaml.Media.SolidColorBrush StatusColor
    {
        get
        {
            var runningText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("DevHub_Running") ?? "Rodando";
            var offlineText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("DevHub_Offline") ?? "Offline";

            return StatusText == runningText ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen) :
                   StatusText == offlineText ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange) :
                   new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DimGray);
        }
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}
