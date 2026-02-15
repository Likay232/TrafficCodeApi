using MauiApp.Infrastructure.Models.DTO;

namespace MauiApp.Infrastructure.Models.Requests;

public class SaveAnswers
{
    public int UserId { get; set; }
    public bool IsPassed { get; set; }
    public int MistakesCount { get; set; }
    public List<UserAnswer> UserAnswers { get; set; } = [];
}