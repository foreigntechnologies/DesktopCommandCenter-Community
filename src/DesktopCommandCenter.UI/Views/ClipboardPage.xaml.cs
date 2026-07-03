using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ClipboardPage : Page
{
    public ViewModels.ClipboardViewModel ViewModel { get; }

    public ClipboardPage()
    {
InitializeComponent();
            UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.ClipboardViewModel>();
        
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.Activate();
        _ = ViewModel.LoadItemsAsync();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.Deactivate();
    }

        private void UpdateTranslations()
        {
            ClipPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Clip_PageTitle");
            if (ClipBtnClearElement.Content is string || ClipBtnClearElement.Content == null) ClipBtnClearElement.Content = Helpers.LocalizationHelper.Instance.GetString("Clip_BtnClear");
            
            
        }
}


