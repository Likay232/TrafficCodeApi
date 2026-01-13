namespace WebApi.Infrastructure.Models.DTO;

public class UserDto
{
    public int Id { get; set; }
    
    public string LastName { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    
    public bool IsBlocked { get; set; }
}