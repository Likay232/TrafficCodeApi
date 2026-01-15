namespace WebApi.Infrastructure.Models.DTO;

public class UserDto
{
    public int Id { get; set; }
    
    public string LastName { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public bool IsBlocked { get; set; }
    public DateTime LastLogin { get; set; }

    public List<ThemesStatistic> ThemesStatistics { get; set; } = new ();
}