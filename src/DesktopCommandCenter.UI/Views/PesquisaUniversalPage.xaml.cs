using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class PesquisaUniversalPage : Page
{
    public ViewModels.PesquisaUniversalViewModel ViewModel { get; }

    public PesquisaUniversalPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.PesquisaUniversalViewModel>();
        this.InitializeComponent();
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is ViewModels.SearchResultItem item)
        {
            ViewModel.SelectedItem = item;
            
            // Auto execute if it's an app for quick navigation
            if (item.Type == "App")
            {
                ExecuteAction(item);
            }
        }
    }

    private void BtnOpen_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.SelectedItem != null)
        {
            ExecuteAction(ViewModel.SelectedItem);
        }
    }

    private void BtnSendToAI_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.SelectedItem != null)
        {
            // Set the selected file/path to the AI context and navigate
            var iaViewModel = App.Current.Services.GetRequiredService<ViewModels.IALocalViewModel>();
            iaViewModel.AttachedImagePath = ViewModel.SelectedItem.ActionPath;
            this.Frame.Navigate(typeof(IALocalPage));
        }
    }

    private void ExecuteAction(ViewModels.SearchResultItem item)
    {
        if (item.Type == "App")
        {
            switch (item.ActionPath)
            {
                case "Calculadora": this.Frame.Navigate(typeof(CalculadoraPage)); break;
                case "AlwaysOnTop": this.Frame.Navigate(typeof(AlwaysOnTopPage)); break;
                case "Awake": this.Frame.Navigate(typeof(AwakePage)); break;
                case "ColorPicker": this.Frame.Navigate(typeof(ColorPickerPage)); break;
                case "ChatFT": this.Frame.Navigate(typeof(IALocalPage)); break;
            }
        }
        else
        {
            ViewModel.ExecuteItem(item);
        }
    }
}
