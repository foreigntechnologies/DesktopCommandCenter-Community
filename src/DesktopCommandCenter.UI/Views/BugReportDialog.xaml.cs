using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class BugReportDialog : ContentDialog
{
    private string? _attachedFilePath = null;

    public BugReportDialog()
    {

        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    private async void AttachButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker();
            // Para WinUI 3, precisamos passar o HWND para o picker
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _attachedFilePath = file.Path;
                AttachmentNameText.Text = file.Name;
                AttachmentNameText.Foreground = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources["TextFillColorPrimaryBrush"];
            }
        }
        catch (Exception ex)
        {
            StatusInfoBar.IsOpen = true;
            StatusInfoBar.Severity = InfoBarSeverity.Error;
            StatusInfoBar.Title = "Erro ao anexar";
            StatusInfoBar.Message = ex.Message;
        }
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true; // Impede o dialog de fechar enquanto envia

        if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            StatusInfoBar.IsOpen = true;
            StatusInfoBar.Severity = InfoBarSeverity.Warning;
            StatusInfoBar.Title = "Campos obrigatÃ³rios";
            StatusInfoBar.Message = "Por favor, preencha o tÃ­tulo e a descriÃ§Ã£o do problema.";
            return;
        }

        StatusInfoBar.IsOpen = true;
        StatusInfoBar.Severity = InfoBarSeverity.Informational;
        StatusInfoBar.Title = "Enviando...";
        StatusInfoBar.Message = "Aguarde enquanto transmitimos o seu reporte.";
        
        // Desabilita botÃµes
        IsPrimaryButtonEnabled = false;

        try
        {
            // PadrÃ£o: Preparando o payload para ser enviado a um Webhook do Firebase ou API de Email
            var payload = new
            {
                title = TitleTextBox.Text,
                description = DescriptionTextBox.Text,
                attachmentPath = _attachedFilePath,
                appVersion = App.IsProBuild ? "PRO" : "COMMUNITY",
                timestamp = DateTime.UtcNow.ToString("o")
            };

            // Aqui vocÃª pode inserir a URL real do seu Firebase Function ou MailerSend
            // string webhookUrl = "https://us-central1-SEU-PROJETO.cloudfunctions.net/reportBug";
            
            // SimulaÃ§Ã£o de envio para a API (Remover o Task.Delay na integraÃ§Ã£o real)
            await Task.Delay(1500); 

            // Se for usar HttpClient real:
            /*
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(webhookUrl, content);
            response.EnsureSuccessStatusCode();
            */

            StatusInfoBar.Severity = InfoBarSeverity.Success;
            StatusInfoBar.Title = "Enviado com sucesso!";
            StatusInfoBar.Message = "Obrigado por contribuir. Nossa equipe analisarÃ¡ em breve.";
            
            // Aguarda um momento para o usuÃ¡rio ler a mensagem e fecha
            await Task.Delay(2000);
            this.Hide();
        }
        catch (Exception ex)
        {
            StatusInfoBar.Severity = InfoBarSeverity.Error;
            StatusInfoBar.Title = "Erro ao enviar";
            StatusInfoBar.Message = "Verifique sua conexÃ£o e tente novamente. Detalhes: " + ex.Message;
            IsPrimaryButtonEnabled = true;
        }
    }

        private void UpdateTranslations()
        {
            BugReportTitleElement.Title = Helpers.LocalizationHelper.Instance.GetString("BugReport_Title");
            BugReportDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("BugReport_Desc");
            TitleTextBox.Header = Helpers.LocalizationHelper.Instance.GetString("BugReport_InputTitle");
            var p_TitleTextBox = Helpers.LocalizationHelper.Instance.GetString("BugReport_InputTitle_Placeholder");
            if (!string.IsNullOrEmpty(p_TitleTextBox) && p_TitleTextBox != "BugReport_InputTitle_Placeholder") TitleTextBox.PlaceholderText = p_TitleTextBox;
            DescriptionTextBox.Header = Helpers.LocalizationHelper.Instance.GetString("BugReport_InputDesc");
            var p_DescriptionTextBox = Helpers.LocalizationHelper.Instance.GetString("BugReport_InputDesc_Placeholder");
            if (!string.IsNullOrEmpty(p_DescriptionTextBox) && p_DescriptionTextBox != "BugReport_InputDesc_Placeholder") DescriptionTextBox.PlaceholderText = p_DescriptionTextBox;
            BugReportAttachTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("BugReport_AttachTitle");
            AttachmentNameText.Text = Helpers.LocalizationHelper.Instance.GetString("BugReport_NoFile");
            if (BugReportBtnBrowseElement.Content is string || BugReportBtnBrowseElement.Content == null) BugReportBtnBrowseElement.Content = Helpers.LocalizationHelper.Instance.GetString("BugReport_BtnBrowse");
        }
}


