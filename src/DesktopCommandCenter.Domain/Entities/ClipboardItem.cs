namespace DesktopCommandCenter.Domain.Entities;

public class ClipboardItem : EntityBase
{
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "Text";
    public bool IsPinned { get; set; }
}
