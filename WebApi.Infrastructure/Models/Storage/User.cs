namespace WebApi.Infrastructure.Models.Storage;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public DateTime LastLogin { get; set; }

    public bool IsBlocked { get; set; }
    
    public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    public ICollection<CompletedTask> CompletedTasks { get; set; } = new List<CompletedTask>();
    public ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();
    public ICollection<TestUser> TestUsers { get; set; } = new List<TestUser>();
}