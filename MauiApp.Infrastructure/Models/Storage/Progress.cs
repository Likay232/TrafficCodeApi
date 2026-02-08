namespace MauiApp.Infrastructure.Models.Storage;

public class Progress : BaseEntity
{
    public int UserId { get; set; }
    public int ThemeId { get; set; }
    
    public int Level { get; set; }
    public double AmountToLevelUp { get; set; }
}