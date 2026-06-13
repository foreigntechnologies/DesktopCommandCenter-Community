using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly ILicenseService _licenseService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool _isLoading;

    public bool IsNotLoading => !IsLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _statusMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(StatusMessage) && !IsLoading && !StatusMessage.Contains("sucesso", StringComparison.OrdinalIgnoreCase);

    public bool HasSuccess => !string.IsNullOrEmpty(StatusMessage) && !IsLoading && StatusMessage.Contains("sucesso", StringComparison.OrdinalIgnoreCase);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
    [NotifyPropertyChangedFor(nameof(IsFreePlan))]
    [NotifyPropertyChangedFor(nameof(IsProPlan))]
    private bool _isLoggedIn;

    public bool IsNotLoggedIn => !IsLoggedIn;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFreePlan))]
    [NotifyPropertyChangedFor(nameof(IsProPlan))]
    [NotifyPropertyChangedFor(nameof(PlanDisplayText))]
    private string _currentPlan = "free";

    [ObservableProperty]
    private string _userEmail = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLinkGoogle))]
    [NotifyPropertyChangedFor(nameof(HasNoGoogleLinked))]
    private bool _hasGoogleLinked;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLinkGitHub))]
    private bool _hasGitHubLinked;

    [ObservableProperty]
    private string _linkedEmailsText = string.Empty;

    [ObservableProperty]
    private string _googleEmail = string.Empty;

    [ObservableProperty]
    private string _gitHubEmail = string.Empty;

    public bool CanLinkGoogle => !HasGoogleLinked;
    public bool CanLinkGitHub => !HasGitHubLinked;
    public bool HasNoGoogleLinked => !HasGoogleLinked;

    public bool IsFreePlan => IsLoggedIn && !CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);
    public bool IsProPlan  => IsLoggedIn && CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);
    public string PlanDisplayText => IsProPlan ? "✔ Plano PRO Enterprise ativo" : "Plano Community (Gratuito)";

    public AuthViewModel(IAuthService authService, ILicenseService licenseService)
    {
        _authService = authService;
        _licenseService = licenseService;
        
        // Verifica o estado atual de login e licença em background ao iniciar a ViewModel
        _ = CheckInitialStateAsync();
    }
    
    private async Task CheckInitialStateAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user != null)
        {
            await OnLoginSuccessAsync(user);
        }
        else
        {
            CurrentPlan = "free";
            StatusMessage = string.Empty;
        }
    }

    // ── Helper compartilhado pós-login ──────────────────────────────────────
    private async Task OnLoginSuccessAsync(Application.Interfaces.AuthUser user)
    {
        var plan = await _licenseService.GetCurrentPlanAsync();
        
        App.Current.MainWindow?.DispatcherQueue.TryEnqueue(() =>
        {
            CurrentPlan = plan;
            App.IsProUnlocked = App.IsProBuild && CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);
            WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(App.IsProUnlocked));
            UserEmail  = user.Email;
            HasGoogleLinked = user.Providers.Contains("google.com");
            HasGitHubLinked = user.Providers.Contains("github.com");
            
            if (HasGoogleLinked && user.LinkedEmails.TryGetValue("google.com", out var gEmail)) GoogleEmail = gEmail; else GoogleEmail = string.Empty;
            if (HasGitHubLinked && user.LinkedEmails.TryGetValue("github.com", out var ghEmail)) GitHubEmail = ghEmail; else GitHubEmail = string.Empty;

            IsLoggedIn = true;
            StatusMessage = string.Empty;

            // Restaura a janela e traz para frente após o login via browser
            App.Current.MainWindow?.AppWindow.Show();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow);
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        });
    }

    [RelayCommand]
    public async Task LoginWithGoogleAsync()
    {
        if (IsLoading) return;
        StatusMessage = string.Empty;
        IsLoading = true;
        try
        {
            var user = await _authService.LoginWithGoogleAsync();
            await OnLoginSuccessAsync(user);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task LoginWithGitHubAsync()
    {
        if (IsLoading) return;
        StatusMessage = string.Empty;
        IsLoading = true;
        try
        {
            var user = await _authService.LoginWithGitHubAsync();
            await OnLoginSuccessAsync(user);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task LinkGoogleAsync()
    {
        if (IsLoading) return;
        StatusMessage = string.Empty;
        IsLoading = true;
        try
        {
            var user = await _authService.LinkWithGoogleAsync();
            StatusMessage = "Conta do Google vinculada com sucesso!";
            await OnLoginSuccessAsync(user);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao vincular: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task LinkGitHubAsync()
    {
        if (IsLoading) return;
        StatusMessage = string.Empty;
        IsLoading = true;
        try
        {
            var user = await _authService.LinkWithGitHubAsync();
            StatusMessage = "Conta do GitHub vinculada com sucesso!";
            await OnLoginSuccessAsync(user);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao vincular: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public void Logout()
    {
        _authService.Logout();
        IsLoggedIn    = false;
        CurrentPlan   = "free";
        UserEmail     = string.Empty;
        LinkedEmailsText = string.Empty;
        GoogleEmail   = string.Empty;
        GitHubEmail   = string.Empty;
        HasGoogleLinked = false;
        HasGitHubLinked = false;
        StatusMessage = string.Empty;
        App.IsProUnlocked = false;
        WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(App.IsProUnlocked));
    }

    [RelayCommand]
    public async Task UpgradeMonthlyAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null) return;
        
        // O UID é passado no client_reference_id para o webhook atualizar o banco!
        string url = $"https://buy.stripe.com/14AeVf9Q46Gz5nY9ttf3a0p?client_reference_id={user.Uid}";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }

    [RelayCommand]
    public async Task UpgradeYearlyAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null) return;
        
        string url = $"https://buy.stripe.com/7sYbJ3e6k3uncQq499f3a0q?client_reference_id={user.Uid}";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }

    [RelayCommand]
    public void OpenCustomerPortal()
    {
        // Redireciona para o Customer Portal do Stripe
        string url = "https://billing.stripe.com/p/login/SEU_ID_DO_PORTAL";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }
}
