namespace MauiApp.Models.Storage;

public class Progress
{
    public int UserId { get; set; }
    public int ThemeId { get; set; }
    
    public int Level { get; set; }
    public int AmountToLevelUp { get; set; }
    
    public bool IsSynced { get; set; }
}