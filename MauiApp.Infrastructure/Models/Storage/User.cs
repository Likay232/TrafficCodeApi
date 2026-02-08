namespace MauiApp.Infrastructure.Models.Storage;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public DateTime LastLogin { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime RegistrationDate { get; set; }
}