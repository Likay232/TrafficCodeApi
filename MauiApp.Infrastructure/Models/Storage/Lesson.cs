namespace MauiApp.Infrastructure.Models.Storage;

public class Lesson : BaseEntity
{
    public string? Text { get; set; } = string.Empty;

    public string? Link { get; set; } = String.Empty;

    public int ThemeId { get; set; }
}