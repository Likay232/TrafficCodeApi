using MauiApp.Infrastructure.Models.DTO;

namespace MauiApp.Infrastructure.Models.Requests;

public class SaveAnswers
{
    public int UserId { get; set; }
    public List<UserAnswer> UserAnswers { get; set; } = [];
}