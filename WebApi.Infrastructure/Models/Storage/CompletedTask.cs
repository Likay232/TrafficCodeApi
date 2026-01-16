namespace WebApi.Infrastructure.Models.Storage;

public class CompletedTask : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int TaskId { get; set; }
    public Task Task { get; set; } = null!;
    
    public bool? IsCorrect { get; set; }
    public DateTime CompletedAt { get; set; }
}