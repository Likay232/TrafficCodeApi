namespace WebApi.Infrastructure.Models.Storage;

public class Theme : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; } = string.Empty;
    
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}