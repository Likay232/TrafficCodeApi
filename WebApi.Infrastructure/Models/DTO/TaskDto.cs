using Microsoft.AspNetCore.Http;

namespace WebApi.Infrastructure.Models.DTO;

public class TaskDto
{
    public int Id { get; set; }
    
    public int ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    
    public string Text { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public List<string?> AnswerVariants { get; set; } = new List<string?>();
    public string Hint { get; set; } = string.Empty;
    
    public int DifficultyLevel { get; set; }

    public IFormFile? Image { get; set; }   

    public byte[]? ImageData { get; set; }
    public string? FilePath { get; set; }
}