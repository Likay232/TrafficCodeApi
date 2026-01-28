namespace MauiApp.Infrastructure.Models.Storage;

public class Theme : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; } = string.Empty;
}