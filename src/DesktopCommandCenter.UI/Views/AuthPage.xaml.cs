using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AuthPage : Page
{
    public AuthViewModel ViewModel { get; }

    public AuthPage()
    {
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<AuthViewModel>();
        InitializeComponent();
        Loaded += AuthPage_Loaded;
        // Translate.Key on each XAML element handles live language updates automatically.
    }

    private void AuthPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Translations are handled automatically by helpers:Translate.Key on each XAML element.
        // No manual UpdateTranslations() needed here.
    }

    public Microsoft.UI.Xaml.Media.ImageSource? ConvertToImageSource(string url)
    {
        if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return null;
        try
        {
            return new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(uri);
        }
        catch
        {
            return null;
        }
    }

    private async void OpenCommunityRepo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/KoglinDev/DesktopCommandCenter-Community"));
    }
}
