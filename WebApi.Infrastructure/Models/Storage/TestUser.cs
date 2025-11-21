namespace WebApi.Infrastructure.Models.Storage;

public class TestUser
{
    public int TestId { get; set; }
    public Test? Test { get; set; }
    public int UserId { get; set; }
    
    public string Score { get; set; } = string.Empty;
    public DateTime CompletionTime { get; set; } = DateTime.Now;
}