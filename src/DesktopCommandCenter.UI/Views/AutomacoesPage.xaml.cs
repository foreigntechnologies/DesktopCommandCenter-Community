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
    }

    private async void NovaRegraButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TriggersCombo.SelectedIndex = -1;
        ActionsCombo.SelectedIndex = -1;
        NovaRegraDialog.XamlRoot = this.XamlRoot;
        await NovaRegraDialog.ShowAsync();
    }

    private void NovaRegraDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var trigger = TriggersCombo.SelectedItem as string;
        var action = ActionsCombo.SelectedItem as string;
        if (!string.IsNullOrEmpty(trigger) && !string.IsNullOrEmpty(action))
        {
            ViewModel.AddNovaRegra(trigger, action);
        }
    }
}
