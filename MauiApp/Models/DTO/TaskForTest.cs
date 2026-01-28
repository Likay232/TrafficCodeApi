namespace MauiApp.Models;

public class TaskForTest : BaseModel
{
    public string Text { get; set; }
    
    public string CorrectAnswer { get; set; }
    
    public int DifficultyLevel { get; set; }
    
    public byte[]? Image { get; set; }
    
    public string? File { get; set; }
    
    public bool? IsCorrect { get; set; }
}