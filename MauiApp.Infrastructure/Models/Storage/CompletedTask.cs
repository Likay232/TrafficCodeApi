namespace MauiApp.Infrastructure.Models.Storage;

public class CompletedTask : BaseEntity
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public Task Task { get; set; }
    public bool? IsCorrect { get; set; }
    
    public DateTime CompletedAt { get; set; }
}