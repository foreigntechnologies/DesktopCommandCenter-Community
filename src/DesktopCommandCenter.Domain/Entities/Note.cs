namespace DesktopCommandCenter.Domain.Entities;

public class Note : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = "Geral";
    public bool IsPinned { get; set; }
}

