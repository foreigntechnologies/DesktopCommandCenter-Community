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
            var loc = Helpers.LocalizationHelper.Instance;
            NotesPageTitleElement.Text = loc.GetString("Notes_PageTitle");
            AddNoteTextElement.Text = loc.GetString("Notes_AddNote") ?? "Adicionar Nota";
        }

        private async void AddNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await ShowNoteDialogAsync(null);
        }

        private async void EditNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DesktopCommandCenter.Domain.Entities.Note note)
            {
                await ShowNoteDialogAsync(note);
            }
        }

        private async void DeleteNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DesktopCommandCenter.Domain.Entities.Note note)
            {
                var loc = Helpers.LocalizationHelper.Instance;
                var dialog = new ContentDialog
                {
                    Title = loc.GetString("Notes_DeleteConfirmTitle") ?? "Confirmar Exclusão",
                    Content = loc.GetString("Notes_DeleteConfirmContent") ?? "Tem certeza que deseja excluir esta nota?",
                    PrimaryButtonText = loc.GetString("Notes_DeleteNote") ?? "Excluir",
                    CloseButtonText = loc.GetString("Notes_Cancel") ?? "Cancelar",
                    XamlRoot = this.XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await ViewModel.DeleteNoteAsync(note);
                }
            }
        }

        private async System.Threading.Tasks.Task ShowNoteDialogAsync(DesktopCommandCenter.Domain.Entities.Note? noteToEdit)
        {
            var loc = Helpers.LocalizationHelper.Instance;
            bool isEdit = noteToEdit != null;

            var titleBox = new TextBox 
            { 
                PlaceholderText = loc.GetString("Notes_TitlePlaceholder") ?? "Título da nota", 
                Text = isEdit && noteToEdit != null ? noteToEdit.Title : "",
                Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 12)
            };
            
            var categoryBox = new TextBox 
            { 
                PlaceholderText = loc.GetString("Notes_CategoryPlaceholder") ?? "Categoria", 
                Text = isEdit && noteToEdit != null ? noteToEdit.Category : "",
                Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 12)
            };
            
            var contentBox = new TextBox 
            { 
                PlaceholderText = loc.GetString("Notes_ContentPlaceholder") ?? "Escreva sua nota aqui...",
                Text = isEdit && noteToEdit != null ? noteToEdit.Content : "",
                AcceptsReturn = true, 
                MinHeight = 120, 
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap 
            };

            var panel = new StackPanel();
            panel.Children.Add(titleBox);
            panel.Children.Add(categoryBox);
            panel.Children.Add(contentBox);

            var dialog = new ContentDialog
            {
                Title = isEdit 
                    ? (loc.GetString("Notes_EditNote") ?? "Editar Nota")
                    : (loc.GetString("Notes_AddNote") ?? "Adicionar Nota"),
                Content = panel,
                PrimaryButtonText = loc.GetString("Notes_Save") ?? "Salvar",
                CloseButtonText = loc.GetString("Notes_Cancel") ?? "Cancelar",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                string title = string.IsNullOrWhiteSpace(titleBox.Text) ? "Nota sem título" : titleBox.Text;
                string category = string.IsNullOrWhiteSpace(categoryBox.Text) ? "Geral" : categoryBox.Text;
                string content = contentBox.Text;
                
                if (string.IsNullOrWhiteSpace(content)) return;

                if (isEdit && noteToEdit != null)
                {
                    await ViewModel.UpdateNoteAsync(noteToEdit, title, content, category);
                }
                else
                {
                    await ViewModel.CreateNoteAsync(title, content, category);
                }
            }
        }
}


