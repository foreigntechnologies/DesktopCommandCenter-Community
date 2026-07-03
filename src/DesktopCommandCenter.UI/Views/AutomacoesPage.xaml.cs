using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AutomacoesPage : Page
{
    public ViewModels.AutomacoesViewModel ViewModel { get; }

    public AutomacoesPage()
    {
ViewModel = App.Current.Services.GetRequiredService<ViewModels.AutomacoesViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    private async void NovaRegraButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TriggersCombo.SelectedIndex = -1;
        ActionsCombo.SelectedIndex = -1;
        TriggerParamTextBox.Text = string.Empty;
        TriggerParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        LanguageCombo.SelectedIndex = -1;
        LanguageCombo.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        ActionParamTextBox.Text = string.Empty;
        ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

        NovaRegraDialog.XamlRoot = this.XamlRoot;
        await NovaRegraDialog.ShowAsync();
    }

    private void NovaRegraDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var trigger = TriggersCombo.SelectedItem as string;
        var action = ActionsCombo.SelectedItem as string;
        var triggerParam = TriggerParamTextBox.Text;
        var actionParam = ActionParamTextBox.Text;
        var actionLinguagem = LanguageCombo.SelectedItem as string;

        if (!string.IsNullOrEmpty(trigger) && !string.IsNullOrEmpty(action))
        {
            ViewModel.AddNovaRegra(trigger, action, triggerParam ?? "", actionParam ?? "", actionLinguagem ?? "");
        }
    }

    private void TriggersCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var trigger = TriggersCombo.SelectedItem as string;
        if (trigger == "A cada X minutos/horas")
        {
            TriggerParamTextBox.Header = "Intervalo (Minutos)";
            TriggerParamTextBox.PlaceholderText = "Ex: 15";
            TriggerParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(TriggerParamTextBox, "Minutos do Intervalo");
        }
        else if (trigger == "Ao abrir um aplicativo especÃ­fico")
        {
            TriggerParamTextBox.Header = "Nome do Processo / ExecutÃ¡vel";
            TriggerParamTextBox.PlaceholderText = "Ex: devenv.exe ou devenv";
            TriggerParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(TriggerParamTextBox, "Nome do aplicativo a monitorar");
        }
        else
        {
            TriggerParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            TriggerParamTextBox.Text = string.Empty;
        }
    }

    private void ActionsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var action = ActionsCombo.SelectedItem as string;
        
        // Reset defaults
        LanguageCombo.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        ActionParamTextBox.AcceptsReturn = false;
        ActionParamTextBox.TextWrapping = Microsoft.UI.Xaml.TextWrapping.NoWrap;
        ActionParamTextBox.MinHeight = 0;
        
        if (action == "Executar script personalizado")
        {
            LanguageCombo.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            ActionParamTextBox.Header = "CÃ³digo do Script";
            ActionParamTextBox.PlaceholderText = "Cole ou escreva seu script aqui...";
            ActionParamTextBox.AcceptsReturn = true;
            ActionParamTextBox.TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap;
            ActionParamTextBox.MinHeight = 150;
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "CÃ³digo do script personalizado");
        }
        else if (action == "Abrir programa")
        {
            ActionParamTextBox.Header = "Caminho ou nome do executÃ¡vel";
            ActionParamTextBox.PlaceholderText = "Ex: C:\\Windows\\notepad.exe ou notepad";
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Caminho do executÃ¡vel");
        }
        else if (action == "Falar texto (Text-to-Speech)")
        {
            ActionParamTextBox.Header = "Mensagem falada";
            ActionParamTextBox.PlaceholderText = "Ex: Processamento concluÃ­do!";
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Mensagem falada");
        }
        else if (action == "Executar script PowerShell ou CMD")
        {
            ActionParamTextBox.Header = "Caminho completo do script (.ps1 ou .bat)";
            ActionParamTextBox.PlaceholderText = "Ex: C:\\Scripts\\backup.ps1";
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Caminho do script a executar");
        }
        else if (action == "Exibir notificaÃ§Ã£o do sistema (Toast)")
        {
            ActionParamTextBox.Header = "Mensagem da notificaÃ§Ã£o";
            ActionParamTextBox.PlaceholderText = "Ex: Nova automaÃ§Ã£o disparada!";
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Mensagem da notificaÃ§Ã£o");
        }
        else
        {
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            ActionParamTextBox.Text = string.Empty;
        }
    }

        private void UpdateTranslations()
        {
            AutoPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Auto_PageTitle");
            if (AutoBtnNewRuleElement.Content is string || AutoBtnNewRuleElement.Content == null) AutoBtnNewRuleElement.Content = Helpers.LocalizationHelper.Instance.GetString("Auto_BtnNewRule");
            
            
// Removed
            NovaRegraDialog.Title = Helpers.LocalizationHelper.Instance.GetString("Auto_DialogTitle");
            TriggersCombo.Header = Helpers.LocalizationHelper.Instance.GetString("Auto_ComboTrigger");
            ActionsCombo.Header = Helpers.LocalizationHelper.Instance.GetString("Auto_ComboAction");
            LanguageCombo.Header = Helpers.LocalizationHelper.Instance.GetString("Auto_ComboLang");
        }
}




