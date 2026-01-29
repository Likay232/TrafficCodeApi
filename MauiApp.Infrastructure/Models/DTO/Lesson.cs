namespace MauiApp.Infrastructure.Models.DTO;

public class Lesson
{
    public int ThemeId { get; set; }

    public string Text { get; set; } = string.Empty;
    
    public string? Link { get; set; }
}