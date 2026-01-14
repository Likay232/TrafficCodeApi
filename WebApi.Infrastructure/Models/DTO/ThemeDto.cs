namespace WebApi.Infrastructure.Models.DTO;

public class ThemeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}