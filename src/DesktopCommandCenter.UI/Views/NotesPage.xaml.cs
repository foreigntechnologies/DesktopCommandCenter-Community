using Microsoft.UI.Xaml.Controls;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class NotesPage : Page
{
    public NotesViewModel ViewModel { get; }

    public NotesPage()
    {
        ViewModel = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<NotesViewModel>(((App)Microsoft.UI.Xaml.Application.Current).Services);
        InitializeComponent();
        
        Loaded += async (s, e) => await ViewModel.LoadNotesAsync();
    }
}

