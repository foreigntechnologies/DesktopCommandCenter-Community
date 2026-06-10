using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly ILicenseService _licenseService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
    private bool _isLoggedIn;

    public bool IsNotLoggedIn => !IsLoggedIn;

    [ObservableProperty]
    private string _currentPlan = "free";

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
        IsLoggedIn = user != null;
        if (IsLoggedIn)
        {
            CurrentPlan = await _licenseService.GetCurrentPlanAsync();
            StatusMessage = $"Logado como: {user!.Email} (Plano: {CurrentPlan.ToUpper()})";
        }
        else
        {
            CurrentPlan = "free";
            StatusMessage = "Você está usando a versão gratuita offline.";
        }
    }

    [RelayCommand]
    public async Task LoginWithGoogleAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Autenticando via Google...";
            var user = await _authService.LoginWithGoogleAsync();
            
            // Após o login, checa no Firestore se já é PRO
            CurrentPlan = await _licenseService.GetCurrentPlanAsync();
            IsLoggedIn = true;
            
            StatusMessage = $"Bem-vindo, {user.Email}! Plano: {CurrentPlan.ToUpper()}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro no Login Google: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoginWithGitHubAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Autenticando via GitHub...";
            var user = await _authService.LoginWithGitHubAsync();
            
            CurrentPlan = await _licenseService.GetCurrentPlanAsync();
            IsLoggedIn = true;

            StatusMessage = $"Bem-vindo, {user.Email}! Plano: {CurrentPlan.ToUpper()}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro no Login GitHub: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
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
