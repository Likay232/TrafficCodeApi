using MauiApp.Infrastructure.Models.Repositories;
using MauiApp.Infrastructure.Services;

namespace MauiApp.Infrastructure.Models.DTO;

public class TaskForTest : BaseModel
{
    public int ThemeId { get; set; }
    public string Text { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;

    public List<string?> AnswerVariants { get; set; } = new();

    public string Hint { get; set; } = string.Empty;

    public int DifficultyLevel { get; set; }

    public string? FilePath
    {
        set => ImageUrl = AppRepository.GetFileAbsolutePath(value);
    }

    public string? ImageUrl { get; set; }

    public bool? IsCorrect { get; set; }

    public List<AnswerVariant> AnswerVariantsWithFlag { get; set; } = new();

    public void BuildVariants()
    {
        AnswerVariantsWithFlag = AnswerVariants.Select(v => new AnswerVariant
        {
            Text = v ?? "",
            IsCorrect = v == CorrectAnswer
        }).ToList();
    }
}
