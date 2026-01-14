namespace WebApi.Infrastructure.Models.Storage;

public class TestUser : BaseEntity
{
    public int TestId { get; set; }
    public Test? Test { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public string Score { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; } = DateTime.Now;
}