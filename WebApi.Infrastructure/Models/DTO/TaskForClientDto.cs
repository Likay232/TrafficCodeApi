namespace WebApi.Infrastructure.Models.DTO;

public class TaskForClientDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public int DifficultyLevel { get; set; }
    public string? File { get; set; }
    public bool? IsCorrect { get; set; }
}