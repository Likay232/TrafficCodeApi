namespace MauiApp.Models.Storage;

public class CompletedTask
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public bool? IsCorrect { get; set; }
    
    public bool IsSynced { get; set; }
}