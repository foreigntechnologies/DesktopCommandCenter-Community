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
            // NewNoteTitle and NewNoteContent could also be localized here if needed.
        }

        private async void SaveNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            string title = NewNoteTitle.Text;
            string content = NewNoteContent.Text;

            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
                return;

            if (string.IsNullOrWhiteSpace(title))
                title = "Nota sem título";

            await ViewModel.CreateNoteAsync(title, content, "Geral");

            NewNoteTitle.Text = string.Empty;
            NewNoteContent.Text = string.Empty;
        }
}


