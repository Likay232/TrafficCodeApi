namespace WebApi.Infrastructure.Models.DTO;

public class TaskForClientDto
{
    public int Id { get; set; }
    public int ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public List<string?> AnswerVariants { get; set; } = new ();
    public int DifficultyLevel { get; set; }
    public string? FilePath { get; set; }
    public bool? IsCorrect { get; set; }
    public string Hint { get; set; } = string.Empty;
}