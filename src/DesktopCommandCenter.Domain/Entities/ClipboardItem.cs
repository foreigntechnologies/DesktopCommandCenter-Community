namespace DesktopCommandCenter.Domain.Entities;

public class ClipboardItem : EntityBase
{
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "Text"; // Pode ser "Text" ou "Image"
    public string? ImagePath { get; set; }
    public bool IsPinned { get; set; }
}
