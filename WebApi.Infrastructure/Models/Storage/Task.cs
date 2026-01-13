namespace WebApi.Infrastructure.Models.Storage;

public class Task : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    
    // Варианты ответов - не больше 4.
    public string AnswerVariants { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;

    public int DifficultyLevel { get; set; }
    
    public string? FilePath { get; set; }
    
    public int ThemeId { get; set; }
    public Theme Theme { get; set; } = null!;
}