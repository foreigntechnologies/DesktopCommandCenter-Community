using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class SystemUpdatesPage : Page
{
    public SystemUpdatesViewModel ViewModel { get; }

    public SystemUpdatesPage()
    {
var deepCleanService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IDeepCleanService>(App.Current.Services);
        ViewModel = new SystemUpdatesViewModel(deepCleanService);
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        this.Loaded += SystemUpdatesPage_Loaded;
    }

    private async void SystemUpdatesPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Garante que o idioma terminou de carregar antes de exibir status traduzido
        await Helpers.LocalizationHelper.Instance.WhenReady;

        UpdateEmptyStates();
        
        if (!ViewModel.WindowsUpdates.Any() && !ViewModel.IsLoadingWindowsUpdates)
        {
            await ViewModel.CheckWindowsUpdatesAsync();
        }
        
        UpdateEmptyStates();
    }

    private void UpdateEmptyStates()
    {
        PnlWUEmpty.Visibility = ViewModel.WindowsUpdates.Any() ? Visibility.Collapsed : Visibility.Visible;
        PnlSWEmpty.Visibility = ViewModel.SoftwareUpdates.Any() ? Visibility.Collapsed : Visibility.Visible;
        PnlAppsEmpty.Visibility = ViewModel.InstalledApps.Any() ? Visibility.Collapsed : Visibility.Visible;
    }

    private async void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = MainPivot.SelectedItem as PivotItem;
        if (selectedItem == PivotApps && !ViewModel.InstalledApps.Any() && !ViewModel.IsLoadingInstalledApps)
        {
            await ViewModel.LoadInstalledAppsAsync();
            UpdateEmptyStates();
        }
        else if (selectedItem == PivotSW && !ViewModel.SoftwareUpdates.Any() && !ViewModel.IsLoadingSoftwareUpdates)
        {
            await ViewModel.CheckSoftwareUpdatesAsync();
            UpdateEmptyStates();
        }
    }

    // -- ABA 1: Windows Update --

    private async void BtnCheckWU_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.CheckWindowsUpdatesAsync();
        UpdateEmptyStates();
    }

    private async void BtnInstallWU_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.InstallSelectedWindowsUpdatesAsync();
        UpdateEmptyStates();
    }

    private void ToggleAutoInstall_Toggled(object sender, RoutedEventArgs e)
    {
    }

    // -- ABA 2: Softwares (Winget) --

    private async void BtnCheckSW_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.CheckSoftwareUpdatesAsync();
        UpdateEmptyStates();
    }

    private async void BtnUpdateSW_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.UpdateSelectedSoftwareAsync();
        UpdateEmptyStates();
    }

    // -- ABA 3: Gerenciar Apps (DesinstalaÃ§Ã£o) --

    private async void BtnLoadApps_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadInstalledAppsAsync();
        UpdateEmptyStates();
    }

    private async void BtnUninstall_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.UninstallSelectedAppsAsync();
        UpdateEmptyStates();
    }

        private void UpdateTranslations()
        {
            TxtPageTitle.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_PageTitle");
            TxtPageSubtitle.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_PageSubtitle");
            // TODO: Implement translation for PivotWU of type PivotItem
            TxtBtnCheckWU.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_BtnCheckUpdates");
            TxtBtnInstallWU.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_BtnInstallUpdates");
            TxtAutoInstallTitle.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_AutoInstallTitle");
            TxtAutoInstallDesc.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_AutoInstallDesc");
            TxtWUEmptyMsg.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_EmptyWUMsg");
            
            // TODO: Implement translation for PivotSW of type PivotItem
            TxtBtnCheckSW.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_BtnCheckSW");
            TxtBtnUpdateSW.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_BtnUpdateSW");
            TxtSWEmptyMsg.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_EmptySWMsg");
            // TODO: Implement translation for PivotApps of type PivotItem
            SystemUpdatesDeepCleanTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_DeepCleanTitle");
            SystemUpdatesDeepCleanDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_DeepCleanDesc");
            TxtBtnLoadApps.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_BtnLoadApps");
            TxtBtnUninstall.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_BtnUninstall");
            TxtAppsEmptyMsg.Text = Helpers.LocalizationHelper.Instance.GetString("SystemUpdates_EmptyAppsMsg");
        }
}



