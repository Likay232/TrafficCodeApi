namespace MauiApp.Infrastructure.Models.DTO;

public class UserAnswer
{
    public int TaskId { get; set; }
    public string? Answer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}