namespace MauiApp.Infrastructure.Models.DTO;

public class Test
{
    public List<TaskForTest> Tasks { get; set; } = new();
    public Dictionary<int, List<TaskForTest>> AdditionalQuestions { get; set; } = new();
}