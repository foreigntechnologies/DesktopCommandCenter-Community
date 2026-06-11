namespace DesktopCommandCenter.Domain.Entities;

public class Prompt : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
