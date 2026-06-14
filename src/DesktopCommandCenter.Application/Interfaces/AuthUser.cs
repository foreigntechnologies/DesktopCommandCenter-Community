namespace DesktopCommandCenter.Application.Interfaces;

public class AuthUser
{
    public string Uid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public System.Collections.Generic.List<string> Providers { get; set; } = new();
    public string PhotoUrl { get; set; } = string.Empty;
    public System.Collections.Generic.Dictionary<string, string> LinkedEmails { get; set; } = new();
}
