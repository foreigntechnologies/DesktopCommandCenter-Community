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
    }

    private void PasswordInput_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            ViewModel.Password = passwordBox.Password;
        }
    }
}
