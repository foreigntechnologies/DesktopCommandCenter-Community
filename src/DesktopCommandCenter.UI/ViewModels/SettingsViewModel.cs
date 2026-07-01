using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.UI.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IHotkeyConfigManager _hotkeyManager;
    private readonly IAuthService _authService;
    private readonly ILicenseService _licenseService;

    // ── Aparência ──────────────────────────────────────────────────────────────

    [ObservableProperty]
    private int _selectedThemeIndex;

    [ObservableProperty]
    private int _selectedTimeFormatIndex;

    [ObservableProperty]
    private int _selectedLanguageIndex;

    // ── Configurações de IA ──────────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsApiKeyVisible))]
    [NotifyPropertyChangedFor(nameof(ModelPlaceholderText))]
    private int _selectedAIProviderIndex;

    [ObservableProperty]
    private string _aiAgentApiKey = string.Empty;

    [ObservableProperty]
    private string _openAIApiKey = string.Empty;

    [ObservableProperty]
    private string _geminiApiKey = string.Empty;

    [ObservableProperty]
    private string _claudeApiKey = string.Empty;

    [ObservableProperty]
    private string _aiAgentModel = string.Empty;

    public bool IsApiKeyVisible => SelectedAIProviderIndex != 0;

    public string ModelPlaceholderText => SelectedAIProviderIndex switch
    {
        0 => "Deixe em branco para o padrão local (llama3)",
        1 => "Deixe em branco para o padrão (gpt-4o)",
        2 => "Deixe em branco para o padrão (gemini-1.5-pro)",
        3 => "Deixe em branco para o padrão (claude-3-5-sonnet-20240620)",
        _ => "Deixe em branco para o padrão"
    };

    [ObservableProperty]
    private int _selectedDateFormatIndex;

    // ── Hotkeys ────────────────────────────────────────────────────────────────

    public System.Collections.ObjectModel.ObservableCollection<HotkeyConfigItemViewModel> Hotkeys { get; } = new();

    // ── Conta / Assinatura ─────────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
    [NotifyPropertyChangedFor(nameof(IsFreePlan))]
    [NotifyPropertyChangedFor(nameof(IsProPlan))]
    [NotifyPropertyChangedFor(nameof(PlanDisplayText))]
    [NotifyPropertyChangedFor(nameof(PlanBadgeText))]
    [NotifyPropertyChangedFor(nameof(PlanBadgeColor))]
    [NotifyPropertyChangedFor(nameof(InverseProVisibility))]
    private bool _isLoggedIn;

    public bool IsNotLoggedIn => !IsLoggedIn;
    public Visibility InverseProVisibility => IsProPlan ? Visibility.Collapsed : Visibility.Visible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFreePlan))]
    [NotifyPropertyChangedFor(nameof(IsProPlan))]
    [NotifyPropertyChangedFor(nameof(PlanDisplayText))]
    [NotifyPropertyChangedFor(nameof(PlanBadgeText))]
    [NotifyPropertyChangedFor(nameof(PlanBadgeColor))]
    [NotifyPropertyChangedFor(nameof(InverseProVisibility))]
    private string _currentPlan = "free";

    [ObservableProperty]
    private string _userEmail = string.Empty;

    [ObservableProperty]
    private string _userName = string.Empty;

    public bool IsFreePlan => IsLoggedIn && !CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);
    public bool IsProPlan  => IsLoggedIn && CurrentPlan.Equals("pro", StringComparison.OrdinalIgnoreCase);

    public string PlanDisplayText => IsProPlan
        ? "✔ Plano PRO ativo"
        : "Plano Community (Gratuito)";

    public string PlanBadgeText => IsProPlan ? "PRO" : "FREE";

    public string PlanBadgeColor => IsProPlan ? "#7B2FBE" : "#444444";

    // ──────────────────────────────────────────────────────────────────────────

    public SettingsViewModel(
        IHotkeyConfigManager hotkeyManager,
        IAuthService authService,
        ILicenseService licenseService)
    {
        _hotkeyManager = hotkeyManager;
        _authService   = authService;
        _licenseService = licenseService;

        // Aparência
        string themeStr = App.GetTheme();
        SelectedThemeIndex = themeStr == "Light" ? 0 : themeStr == "Dark" ? 1 : 2;

        string appLanguage = App.GetAppLanguage();
        SelectedLanguageIndex = appLanguage switch
        {
            "en-US" => 1,
            "es-ES" => 2,
            _ => 0
        };

        string timeFormat = App.GetTimeFormat();
        SelectedTimeFormatIndex = timeFormat switch
        {
            "HH:mm"      => 0,
            "HH:mm:ss"   => 1,
            "hh:mm tt"   => 2,
            "hh:mm:ss tt"=> 3,
            _            => 0
        };

        string dateFormat = App.GetDateFormat();
        SelectedDateFormatIndex = dateFormat switch
        {
            "dddd, dd MMMM yyyy" => 0,
            "dd/MM/yyyy"         => 1,
            "yyyy-MM-dd"         => 2,
            "MMM d, yyyy"        => 3,
            _                    => 0
        };

        LoadHotkeys();

        // Configurações de IA
        string aiProvider = App.GetAIAgentProvider();
        SelectedAIProviderIndex = aiProvider switch
        {
            "Ollama" => 0,
            "OpenAI" => 1,
            "Gemini" => 2,
            "Claude" => 3,
            _        => 0
        };
        AiAgentApiKey = App.GetAIAgentApiKey();
        OpenAIApiKey = App.GetOpenAIApiKey();
        GeminiApiKey = App.GetGeminiApiKey();
        ClaudeApiKey = App.GetClaudeApiKey();
        AiAgentModel = App.GetAIAgentModel();

        // Escuta mudanças de licença do AuthViewModel (ex: login, logout)
        WeakReferenceMessenger.Default.Register<LicenseChangedMessage>(this, (r, m) =>
        {
            _ = RefreshAccountStateAsync();
        });

        // Set initial synchronous state based on cached data
        IsLoggedIn = !string.IsNullOrEmpty(App.GetCachedEmail());
        UserEmail = App.GetCachedEmail();
        CurrentPlan = App.GetProCached() ? "pro" : "free";

        _ = RefreshAccountStateAsync();
    }

    // ── Hotkeys ────────────────────────────────────────────────────────────────

    private static readonly string[] ProActionIds =
    {
        "ChatFT", "PesquisaUniversal", "Prompts", "Automacoes", "Marketplace"
    };

    private void LoadHotkeys()
    {
        Hotkeys.Clear();
        foreach (var config in _hotkeyManager.GetAllConfigs())
        {
            bool isPro = ProActionIds.Contains(config.ActionId);
            var localizedName = DesktopCommandCenter.UI.App.Current.Services.GetService(typeof(DesktopCommandCenter.Application.Interfaces.ITranslationService)) as DesktopCommandCenter.Application.Interfaces.ITranslationService;
            string display = localizedName?.GetString($"Nav_{config.ActionId}") ?? config.DisplayName;
            // Fallback for settings if needed
            if (display == $"Nav_{config.ActionId}") display = localizedName?.GetString($"Settings_{config.ActionId}") ?? config.DisplayName;

            Hotkeys.Add(new HotkeyConfigItemViewModel
            {
                ActionId             = config.ActionId,
                DisplayName          = display,
                Modifiers            = config.Modifiers,
                VirtualKey           = config.VirtualKey,
                CurrentHotkeyDisplay = DesktopCommandCenter.Infrastructure.Services.GlobalHotkeyService.GetHotkeyString(config.Modifiers, config.VirtualKey),
                IsProFeature         = isPro,
                IsEnabled            = !isPro || App.IsProUnlocked
            });
        }
    }

    public void SaveHotkey(HotkeyConfigItemViewModel item, uint modifiers, uint virtualKey)
    {
        _hotkeyManager.SaveConfig(item.ActionId, modifiers, virtualKey);
        item.Modifiers            = modifiers;
        item.VirtualKey           = virtualKey;
        item.CurrentHotkeyDisplay = DesktopCommandCenter.Infrastructure.Services.GlobalHotkeyService.GetHotkeyString(modifiers, virtualKey);
    }

    // ── Conta / Assinatura ─────────────────────────────────────────────────────

    private async Task RefreshAccountStateAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        IsLoggedIn = user != null;

        if (IsLoggedIn)
        {
            UserEmail   = user!.Email;
            UserName    = user!.DisplayName;
            CurrentPlan = await _licenseService.GetCurrentPlanAsync();
        }
        else
        {
            UserEmail   = string.Empty;
            UserName    = string.Empty;
            CurrentPlan = "free";
        }

        // Atualiza estado dos hotkeys PRO
        foreach (var h in Hotkeys)
            if (h.IsProFeature) h.IsEnabled = App.IsProUnlocked;
    }

    [RelayCommand]
    public void GoToLogin()
    {
        // Navega para a página de Conta usando o mesmo sistema de mensagens da app
        WeakReferenceMessenger.Default.Send(new NavigateMessage("Auth"));
    }

    [RelayCommand]
    public async Task UpgradeMonthly()
    {
        var user = await _authService.GetCurrentUserAsync();
        string uid = user?.Uid ?? "anonymous";
        string url = $"https://buy.stripe.com/14AeVf9Q46Gz5nY9ttf3a0p?client_reference_id={uid}";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }

    [RelayCommand]
    public async Task UpgradeYearly()
    {
        var user = await _authService.GetCurrentUserAsync();
        string uid = user?.Uid ?? "anonymous";
        string url = $"https://buy.stripe.com/7sYbJ3e6k3uncQq499f3a0q?client_reference_id={uid}";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }

    [RelayCommand]
    public void OpenCustomerPortal()
    {
        string url = "https://billing.stripe.com/p/login/7sY7sN6DS9SL5nY6hhf3a00";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }

    [RelayCommand]
    public void Logout()
    {
        _authService.Logout();
        App.SaveCachedEmail(string.Empty);
        IsLoggedIn  = false;
        UserEmail   = string.Empty;
        CurrentPlan = "free";
        App.IsProUnlocked = false;
        WeakReferenceMessenger.Default.Send(new LicenseChangedMessage(App.IsProUnlocked));

        foreach (var h in Hotkeys)
            if (h.IsProFeature) h.IsEnabled = false;
    }

    // ── Aparência ──────────────────────────────────────────────────────────────

    partial void OnSelectedThemeIndexChanged(int value)
    {
        string themeStr = value switch { 0 => "Light", 1 => "Dark", _ => "Default" };
        App.SaveTheme(themeStr);
        App.ApplyTheme(themeStr);
    }

    partial void OnSelectedLanguageIndexChanged(int value)
    {
        string lang = value switch { 1 => "en-US", 2 => "es-ES", _ => "pt-BR" };
        App.SaveAppLanguage(lang);
        
        var translationService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetService(typeof(ITranslationService)) as ITranslationService;
        if (translationService != null)
        {
            _ = translationService.SetLanguageAsync(lang);
        }
    }

    partial void OnSelectedTimeFormatIndexChanged(int value)
    {
        string format = value switch
        {
            0 => "HH:mm", 1 => "HH:mm:ss", 2 => "hh:mm tt", 3 => "hh:mm:ss tt", _ => "HH:mm"
        };
        App.SaveTimeFormat(format);
    }

    partial void OnSelectedDateFormatIndexChanged(int value)
    {
        string format = value switch
        {
            0 => "dddd, dd MMMM yyyy", 1 => "dd/MM/yyyy", 2 => "yyyy-MM-dd", 3 => "MMM d, yyyy",
            _ => "dddd, dd MMMM yyyy"
        };
        App.SaveDateFormat(format);
    }

    partial void OnSelectedAIProviderIndexChanged(int value)
    {
        string provider = value switch
        {
            0 => "Ollama",
            1 => "OpenAI",
            2 => "Gemini",
            3 => "Claude",
            _ => "Ollama"
        };
        App.SaveAIAgentProvider(provider);
        WeakReferenceMessenger.Default.Send(new LicenseChangedMessage(App.IsProUnlocked));
    }

    partial void OnAiAgentApiKeyChanged(string value)
    {
        App.SaveAIAgentApiKey(value);
        WeakReferenceMessenger.Default.Send(new LicenseChangedMessage(App.IsProUnlocked));
    }

    partial void OnOpenAIApiKeyChanged(string value)
    {
        App.SaveOpenAIApiKey(value);
        WeakReferenceMessenger.Default.Send(new LicenseChangedMessage(App.IsProUnlocked));
    }

    partial void OnGeminiApiKeyChanged(string value)
    {
        App.SaveGeminiApiKey(value);
        WeakReferenceMessenger.Default.Send(new LicenseChangedMessage(App.IsProUnlocked));
    }

    partial void OnClaudeApiKeyChanged(string value)
    {
        App.SaveClaudeApiKey(value);
        WeakReferenceMessenger.Default.Send(new LicenseChangedMessage(App.IsProUnlocked));
    }

    partial void OnAiAgentModelChanged(string value)
    {
        App.SaveAIAgentModel(value);
    }
}
