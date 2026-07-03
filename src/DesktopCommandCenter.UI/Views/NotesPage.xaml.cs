using Microsoft.UI.Xaml.Controls;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class NotesPage : Page
{
    public NotesViewModel ViewModel { get; }

    public NotesPage()
    {
InitializeComponent();
            UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        ViewModel = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<NotesViewModel>(((App)Microsoft.UI.Xaml.Application.Current).Services);
        
        
        Loaded += async (s, e) => await ViewModel.LoadNotesAsync();
    }

        private void UpdateTranslations()
        {
            NotesPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Notes_PageTitle");
            if (NotesBtnNewElement.Content is string || NotesBtnNewElement.Content == null) NotesBtnNewElement.Content = Helpers.LocalizationHelper.Instance.GetString("Notes_BtnNew");
        }
}


