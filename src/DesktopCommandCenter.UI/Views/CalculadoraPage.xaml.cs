using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class CalculadoraPage : Page
{
    public ViewModels.CalculadoraViewModel ViewModel { get; }
    
    private int _lastValidTabIndex = 0;

    public CalculadoraPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.CalculadoraViewModel>();
        this.InitializeComponent();
        
        UpdateTabLocks();
    }

    private void UpdateTabLocks()
    {
        if (App.IsProUnlocked)
        {
            FisicaTab.Header = FisicaTab.Header.ToString().Replace(" 🔒", "");
            QuimicaTab.Header = QuimicaTab.Header.ToString().Replace(" 🔒", "");
        }
        else
        {
            if (!FisicaTab.Header.ToString().Contains("🔒"))
                FisicaTab.Header = FisicaTab.Header.ToString() + " 🔒";
                
            if (!QuimicaTab.Header.ToString().Contains("🔒"))
                QuimicaTab.Header = QuimicaTab.Header.ToString() + " 🔒";
        }
    }

    private async void CalculatorTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CalculatorTabs.SelectedIndex == 2 || CalculatorTabs.SelectedIndex == 3) // Física ou Química
        {
            if (!App.IsProUnlocked)
            {
                // Revert to last valid tab
                var restrictedIndex = CalculatorTabs.SelectedIndex;
                CalculatorTabs.SelectedIndex = _lastValidTabIndex;
                
                var featureName = restrictedIndex == 2 ? "Física Quântica / Nuclear" : "Química";
                
                var dialog = new ContentDialog
                {
                    Title = "Recurso PRO Necessário",
                    Content = $"As calculadoras avançadas de '{featureName}' estão disponíveis apenas no plano PRO. Use o menu lateral para assinar o PRO e liberar todos os recursos!",
                    CloseButtonText = "Entendi",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
                return;
            }
        }
        
        // Update last valid tab
        _lastValidTabIndex = CalculatorTabs.SelectedIndex;
    }
}
