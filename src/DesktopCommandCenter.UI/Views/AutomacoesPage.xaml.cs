using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AutomacoesPage : Page
{
    public ViewModels.AutomacoesViewModel ViewModel { get; }
    private ViewModels.AutomacaoRegra? _editingRegra = null;

    public AutomacoesPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.AutomacoesViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
        Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    private async void NovaRegraButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _editingRegra = null;
        NovaRegraDialog.Title = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Automations_DialogTitle") ?? "Criar Nova Regra";
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
            if (_editingRegra != null)
            {
                _editingRegra.Gatilho = trigger;
                _editingRegra.Acao = action;
                _editingRegra.GatilhoParametro = triggerParam ?? "";
                _editingRegra.AcaoParametro = actionParam ?? "";
                _editingRegra.AcaoLinguagem = actionLinguagem ?? "";
                ViewModel.SaveRules();
                _editingRegra = null;
            }
            else
            {
                ViewModel.AddNovaRegra(trigger, action, triggerParam ?? "", actionParam ?? "", actionLinguagem ?? "");
            }
        }
    }

    private void EditRegraButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ViewModels.AutomacaoRegra regra)
        {
            _editingRegra = regra;
            TriggersCombo.SelectedItem = regra.Gatilho;
            ActionsCombo.SelectedItem = regra.Acao;
            
            TriggerParamTextBox.Text = regra.GatilhoParametro;
            ActionParamTextBox.Text = regra.AcaoParametro;
            LanguageCombo.SelectedItem = regra.AcaoLinguagem;
            
            NovaRegraDialog.Title = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Automations_DialogTitleEdit") ?? "Editar Regra";
            NovaRegraDialog.XamlRoot = this.XamlRoot;
            _ = NovaRegraDialog.ShowAsync();
        }
    }

    private void TriggersCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var trigger = TriggersCombo.SelectedItem as string;
        if (trigger == "A cada X minutos/horas")
        {
            TriggerParamTextBox.Header = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoTrigger_Interval_Header") ?? "Intervalo (Minutos)";
            TriggerParamTextBox.PlaceholderText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoTrigger_Interval_Placeholder") ?? "Ex: 15";
            TriggerParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(TriggerParamTextBox, "Minutos do Intervalo");
        }
        else if (trigger == "Ao abrir um aplicativo específico")
        {
            TriggerParamTextBox.Header = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoTrigger_Process_Header") ?? "Nome do Processo / Executável";
            TriggerParamTextBox.PlaceholderText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoTrigger_Process_Placeholder") ?? "Ex: devenv.exe ou devenv";
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
            ActionParamTextBox.Header = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Script_Header") ?? "Código do Script";
            ActionParamTextBox.PlaceholderText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Script_Placeholder") ?? "Cole ou escreva seu script aqui...";
            ActionParamTextBox.AcceptsReturn = true;
            ActionParamTextBox.TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap;
            ActionParamTextBox.MinHeight = 150;
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Código do script personalizado");
        }
        else if (action == "Abrir programa")
        {
            ActionParamTextBox.Header = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Exe_Header") ?? "Caminho ou nome do executável";
            ActionParamTextBox.PlaceholderText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Exe_Placeholder") ?? "Ex: C:\\Windows\\notepad.exe ou notepad";
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Caminho do executável");
        }
        else if (action == "Falar texto (Text-to-Speech)")
        {
            ActionParamTextBox.Header = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Speech_Header") ?? "Texto para Falar";
            ActionParamTextBox.PlaceholderText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Speech_Placeholder") ?? "Ex: Tarefa concluída com sucesso!";
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Texto que o sistema irá falar");
        }
        else if (action == "Exibir notificação do sistema (Toast)")
        {
            ActionParamTextBox.Header = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Toast_Header") ?? "Mensagem da Notificação";
            ActionParamTextBox.PlaceholderText = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("AutoAction_Toast_Placeholder") ?? "Ex: O sistema concluiu a análise.";
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(ActionParamTextBox, "Texto da notificação toast");
        }
        else
        {
            ActionParamTextBox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            ActionParamTextBox.Text = string.Empty;
        }
    }

    private void UpdateTranslations()
    {
        
    }
}
