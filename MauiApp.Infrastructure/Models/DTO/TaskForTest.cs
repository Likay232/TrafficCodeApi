using MauiApp.Infrastructure.Services;

namespace MauiApp.Infrastructure.Models.DTO;

public class TaskForTest : BaseModel
{
    public string Text { get; set; } = string.Empty;
    
    public string CorrectAnswer { get; set; } = string.Empty;
    
    public List<string?> AnswerVariants { get; set; } = new List<string?>();
    public string Hint { get; set; } = string.Empty;
    public int DifficultyLevel { get; set; }

    public string? FilePath { set => ImageUrl = ApiService.GetAbsoluteFilePath(value); }
    public string? ImageUrl { get; set; }

    public bool? IsCorrect { get; set; }
    
    public List<AnswerVariant> AnswerVariantsWithFlag =>
        AnswerVariants.Select(v => new AnswerVariant
        {
            Text = v ?? "",
            IsCorrect = v == CorrectAnswer
        }).ToList();
    
}

public class AnswerVariant
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
