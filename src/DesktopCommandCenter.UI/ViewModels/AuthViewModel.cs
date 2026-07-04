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
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(HasSuccess))]
    private bool _isLoading;

    public bool IsNotLoading => !IsLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(HasSuccess))]
    private string _statusMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(StatusMessage) && !IsLoading && !StatusMessage.Contains("sucesso", StringComparison.OrdinalIgnoreCase);

    public bool HasSuccess => !string.IsNullOrEmpty(StatusMessage) && !IsLoading && StatusMessage.Contains("sucesso", StringComparison.OrdinalIgnoreCase);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
    [NotifyPropertyChangedFor(nameof(IsFreePlan))]
    [NotifyPropertyChangedFor(nameof(IsProPlan))]
    [NotifyPropertyChangedFor(nameof(IsPausedPlan))]
    [NotifyPropertyChangedFor(nameof(FreePlanVisibility))]
    [NotifyPropertyChangedFor(nameof(ProPlanVisibility))]
    [NotifyPropertyChangedFor(nameof(PausedPlanVisibility))]
    [NotifyPropertyChangedFor(nameof(PlanDisplayText))]
    private bool _isLoggedIn;

    public bool IsNotLoggedIn => !IsLoggedIn;

    private CancellationTokenSource? _pollingCts;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFreePlan))]
    [NotifyPropertyChangedFor(nameof(IsProPlan))]
    [NotifyPropertyChangedFor(nameof(IsPausedPlan))]
    [NotifyPropertyChangedFor(nameof(FreePlanVisibility))]
    [NotifyPropertyChangedFor(nameof(ProPlanVisibility))]
    [NotifyPropertyChangedFor(nameof(PausedPlanVisibility))]
    [NotifyPropertyChangedFor(nameof(InverseProVisibility))]
    [NotifyPropertyChangedFor(nameof(PlanDisplayText))]
    private string _currentPlan = "free";

    [ObservableProperty]
    private string _userEmail = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUserName))]
    private string _userName = string.Empty;

    public bool HasUserName => !string.IsNullOrEmpty(UserName);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLinkGoogle))]
    [NotifyPropertyChangedFor(nameof(HasNoGoogleLinked))]
    private bool _hasGoogleLinked;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLinkGitHub))]
    private bool _hasGitHubLinked;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLinkMicrosoft))]
    private bool _hasMicrosoftLinked;

    [ObservableProperty]
    private string _linkedEmailsText = string.Empty;

    [ObservableProperty]
    private string _googleEmail = string.Empty;

    [ObservableProperty]
    private string _gitHubEmail = string.Empty;

    [ObservableProperty]
    private string _microsoftEmail = string.Empty;

    [ObservableProperty]
    private string _profilePhotoUrl = string.Empty;

    public bool CanLinkGoogle => !HasGoogleLinked;
    public bool CanLinkGitHub => !HasGitHubLinked;
    public bool CanLinkMicrosoft => !HasMicrosoftLinked;
    public bool HasNoGoogleLinked => !HasGoogleLinked;

    public bool IsFreePlan => IsLoggedIn && !CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase) && !CurrentPlan.Equals("paused", StringComparison.OrdinalIgnoreCase);
    public bool IsProPlan  => IsLoggedIn && CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);
    public bool IsPausedPlan => IsLoggedIn && CurrentPlan.Equals("paused", StringComparison.OrdinalIgnoreCase);
    
    public Microsoft.UI.Xaml.Visibility FreePlanVisibility => IsFreePlan ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
    public Microsoft.UI.Xaml.Visibility ProPlanVisibility => IsProPlan ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
    public Microsoft.UI.Xaml.Visibility PausedPlanVisibility => IsPausedPlan ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
    public Microsoft.UI.Xaml.Visibility InverseProVisibility => IsProPlan ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible;
    
    public string PlanDisplayText => IsProPlan ? DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_PlanProActive") : (IsPausedPlan ? DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_PlanPaused") : DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_PlanCommunity"));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProPlanPriceText))]
    [NotifyPropertyChangedFor(nameof(ProPlanPeriodText))]
    private bool _isYearlyPlan = false;

    public string ProPlanPriceText => IsYearlyPlan ? "R$ 429,90" : "R$ 39,90";
    public string ProPlanPeriodText => IsYearlyPlan ? "/ ano" : "/ mês";

    public AuthViewModel(IAuthService authService, ILicenseService licenseService)
    {
        _authService = authService;
        _licenseService = licenseService;
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        
        // Escuta mudanças de licença do sistema (ex: ao voltar do Stripe)
        WeakReferenceMessenger.Default.Register<Messages.LicenseChangedMessage>(this, (r, m) =>
        {
            // Apenas atualiza a UI se o valor for diferente, para evitar loop infinito
            if (App.IsProUnlocked != m.Value) 
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    CurrentPlan = m.Value ? "pro" : "free";
                });
            }
        });

        // Set initial synchronous state based on cached data to avoid logged-out UI flickering
        IsLoggedIn = !string.IsNullOrEmpty(App.GetCachedEmail());
        UserEmail = App.GetCachedEmail();
        CurrentPlan = App.GetProCached() ? "pro" : "free";

        // Verifica o estado atual de login e licença em background ao iniciar a ViewModel
        _ = CheckInitialStateAsync();
    }
    
    private async Task CheckInitialStateAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user != null)
        {
            await OnLoginSuccessAsync(user, isNewLogin: false);
        }
        else
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                CurrentPlan = "free";
                StatusMessage = string.Empty;
                App.IsProUnlocked = false;
            });
        }
    }

    // ─── Helper compartilhado pós-login ──────────────────────────────────────────────
    private async Task OnLoginSuccessAsync(Application.Interfaces.AuthUser user, bool isNewLogin = true)
    {
        // 1. Reseta o PRO imediatamente (antes da consulta Firestore) APENAS para novos logins
        //    para evitar que o status da conta anterior vaze para a nova sessão.
        if (isNewLogin)
        {
            App.IsProUnlocked = false;
            App.SaveProCached(false);
        }

        var plan = await _licenseService.GetCurrentPlanAsync();
        
        _dispatcherQueue.TryEnqueue(() =>
        {
            CurrentPlan = plan;
            bool newIsPro = CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);
            
            App.IsProUnlocked = newIsPro;
            
            // Notifica listeners APENAS se não estourar loop (mesmo que seja mesmo valor, os listeners como MainPage cuidam de si)
            // Para evitar o loop do CheckInitialStateAsync, removemos temporariamente o listening ou filtramos.
            // Aqui, enviamos sempre que o login atualiza o plano com sucesso.
            WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(App.IsProUnlocked));
            
            UserEmail  = user.Email;
            UserName   = user.DisplayName;
            ProfilePhotoUrl = user.PhotoUrl;
            
            HasGoogleLinked = user.Providers.Contains("google.com");
            HasGitHubLinked = user.Providers.Contains("github.com");
            HasMicrosoftLinked = user.Providers.Contains("microsoft.com") || user.Providers.Contains("hotmail.com");
            
            if (HasGoogleLinked && user.LinkedEmails.TryGetValue("google.com", out var gEmail)) GoogleEmail = gEmail; else GoogleEmail = string.Empty;
            if (HasGitHubLinked && user.LinkedEmails.TryGetValue("github.com", out var ghEmail)) GitHubEmail = ghEmail; else GitHubEmail = string.Empty;
            if (HasMicrosoftLinked && user.LinkedEmails.TryGetValue("microsoft.com", out var msEmail)) MicrosoftEmail = msEmail; else MicrosoftEmail = string.Empty;

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
    public async Task LoginWithMicrosoftAsync()
    {
        if (IsLoading) return;
        StatusMessage = string.Empty;
        IsLoading = true;
        try
        {
            var user = await _authService.LoginWithMicrosoftAsync();
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
            StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_LinkSuccessGoogle");
            await OnLoginSuccessAsync(user);
        }
        catch (Exception ex)
        {
            StatusMessage = $"{DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_LinkError")}{ex.Message}";
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
            StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_LinkSuccessGitHub");
            await OnLoginSuccessAsync(user);
        }
        catch (Exception ex)
        {
            StatusMessage = $"{DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_LinkError")}{ex.Message}";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task LinkMicrosoftAsync()
    {
        if (IsLoading) return;
        StatusMessage = string.Empty;
        IsLoading = true;
        try
        {
            var user = await _authService.LinkWithMicrosoftAsync();
            StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_LinkSuccessMicrosoft");
            await OnLoginSuccessAsync(user);
        }
        catch (Exception ex)
        {
            StatusMessage = $"{DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_LinkError")}{ex.Message}";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public void Logout()
    {
        _authService.Logout();
        App.SaveCachedEmail(string.Empty);
        IsLoggedIn    = false;
        CurrentPlan   = "free";
        UserEmail     = string.Empty;
        UserName      = string.Empty;
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
    public void CancelLogin()
    {
        _authService.CancelLogin();
        _pollingCts?.Cancel();
        IsLoading = false;
        StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_ProcessInterrupted");
    }

    [RelayCommand]
    public async Task UpgradeMonthlyAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null) return;
        
        bool trialUsed = await _licenseService.HasUsedTrialAsync();
        string url = trialUsed 
            ? $"https://buy.stripe.com/4gM8wRd2gfd54jUgVVf3a0r?client_reference_id={user.Uid}" 
            : $"https://buy.stripe.com/14AeVf9Q46Gz5nY9ttf3a0p?client_reference_id={user.Uid}";
            
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        
        StartPollingForPlanChange("pro");
    }

    [RelayCommand]
    public async Task UpgradeYearlyAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null) return;
        
        bool trialUsed = await _licenseService.HasUsedTrialAsync();
        string url = trialUsed 
            ? $"https://buy.stripe.com/cNi3cx0fud4X3fQ499f3a0s?client_reference_id={user.Uid}" 
            : $"https://buy.stripe.com/7sYbJ3e6k3uncQq499f3a0q?client_reference_id={user.Uid}";

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });

        StartPollingForPlanChange("pro");
    }

    [RelayCommand]
    public async Task SubscribeAsync()
    {
        if (IsYearlyPlan)
            await UpgradeYearlyAsync();
        else
            await UpgradeMonthlyAsync();
    }

    [RelayCommand]
    public async Task OpenCustomerPortalAsync()
    {
        string url = "https://billing.stripe.com/p/login/7sY7sN6DS9SL5nY6hhf3a00";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });

        StartPollingForPlanChange();
    }

    private async void StartPollingForPlanChange(string? expectedNewPlan = null)
    {
        if (IsLoading) return;
        IsLoading = true;
        StatusMessage = expectedNewPlan == "pro" ? DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_WaitingPayment") : DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_WaitingSubChange");
        _pollingCts?.Cancel();
        _pollingCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        string startingPlan = CurrentPlan;

        try 
        {
            while (!_pollingCts.Token.IsCancellationRequested)
            {
                await Task.Delay(3000, _pollingCts.Token);
                var currentDbPlan = await _licenseService.GetCurrentPlanAsync();
                
                // Se o plano no banco mudou em relação ao que tínhamos
                if (currentDbPlan != startingPlan)
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        CurrentPlan = currentDbPlan;
                        App.IsProUnlocked = currentDbPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);
                        WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(App.IsProUnlocked));
                        StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_PlanUpdated");
                        IsLoading = false;
                    });
                    break;
                }
            }
        }
        catch (TaskCanceledException) 
        { 
            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Auth_VerificationInterrupted");
                IsLoading = false;
            });
        }
        catch 
        { 
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsLoading = false;
            });
        }
    }
}
