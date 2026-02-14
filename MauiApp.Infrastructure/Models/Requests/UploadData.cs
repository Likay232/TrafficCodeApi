using MauiApp.Infrastructure.Models.Storage;

namespace MauiApp.Infrastructure.Models.Requests;

public class UploadData
{
    public List<CompletedTask> CompletedTasks { get; set; } = [];
    public List<Progress> Progresses { get; set; } = [];

    public bool HasData()
    {
        return CompletedTasks.Any() || Progresses.Any();
    }

}