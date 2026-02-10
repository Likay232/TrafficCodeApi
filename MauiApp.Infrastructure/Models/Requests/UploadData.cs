using MauiApp.Infrastructure.Models.Storage;
using Task = MauiApp.Infrastructure.Models.Storage.Task;

namespace MauiApp.Infrastructure.Models.Requests;

public class UploadData
{
    public List<CompletedTask> CompletedTasks { get; set; }
    public List<Progress> Progresses { get; set; }

}