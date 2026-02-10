using MauiApp.Infrastructure.Models.Storage;

namespace MauiApp.Infrastructure.Models.Responses;

public class NewData
{
    public List<Lesson> Lessons { get; set; } = [];
    public List<Progress> Progresses { get; set; } = [];
    public List<Storage.Task> Tasks { get; set; } = [];
    public List<Theme> Themes { get; set; } = [];
}