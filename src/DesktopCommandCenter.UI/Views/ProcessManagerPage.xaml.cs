using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.UI.ViewModels;
using DesktopCommandCenter.UI.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ProcessManagerPage : Page
{
    public static Visibility BoolToVis(bool val) => val ? Visibility.Visible : Visibility.Collapsed;

    public ProcessManagerViewModel ViewModel { get; }

    public ProcessManagerPage()
    {
        this.InitializeComponent();
        ViewModel = new ProcessManagerViewModel(App.Current.Services.GetRequiredService<IProcessService>());
        this.Loaded += ProcessManagerPage_Loaded;
        this.Unloaded += ProcessManagerPage_Unloaded;
    }

    private void ProcessManagerPage_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTranslations();
        ViewModel.StartPolling();
    }

    private void ProcessManagerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.StopPolling();
    }

    private void UpdateTranslations()
    {
        var t = LocalizationHelper.Instance;
        
        TxtTitle.Text = t.GetString("ProcMgr_Title");
        BoxSearch.PlaceholderText = t.GetString("ProcMgr_Search");
        TxtHeaderName.Text = t.GetString("ProcMgr_Name");
        TxtHeaderCPU.Text = t.GetString("ProcMgr_CPU");
        TxtHeaderRAM.Text = t.GetString("ProcMgr_RAM");
        TxtHeaderStatus.Text = t.GetString("ProcMgr_Status");

        // The Flyout items and Status labels are inside DataTemplates and Models. 
        // We handle simple properties here, but for DataTemplate strings, it's best handled in ViewModel or via Resource Dictionary in XAML.
        // For simplicity, we assume the Model's "StatusText" translates dynamically, or we use English as fallback.
    }

    private async void FlyoutKillTree_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.Tag is int processId)
        {
            var t = LocalizationHelper.Instance;
            
            ContentDialog dialog = new ContentDialog
            {
                Title = t.GetString("ProcMgr_ConfirmKill_Title") ?? "Confirm Action",
                Content = t.GetString("ProcMgr_ConfirmKill_Msg") ?? "Are you sure you want to terminate this process tree?",
                PrimaryButtonText = t.GetString("ProcMgr_Kill") ?? "Kill",
                CloseButtonText = t.GetString("Gen_Cancel") ?? "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.KillProcessTreeAsync(processId);
            }
        }
    }
}
