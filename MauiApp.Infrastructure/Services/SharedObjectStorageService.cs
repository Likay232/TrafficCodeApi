using MauiApp.Infrastructure.Models.DTO;

namespace MauiApp.Infrastructure.Services;

public class SharedObjectStorageService
{
    public List<TaskForTest>? CurrentTest { get; set; }
    public CheckedTest? CurrentResult { get; set; }
}