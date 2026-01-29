namespace MauiApp.Infrastructure.Models.DTO;

public class CheckedTest
{
    public List<WrongTask> WrongTasks { get; set; } = new();
    public string Score { get; set; } = string.Empty;
}