using System.Collections.ObjectModel;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Services;

namespace MauiApp.ViewModels;

public class TasksViewModel : ViewModelBase<ObservableCollection<TaskForTest>>
{
    public int ThemeId { get; set; }
    
    public TasksViewModel(ApiService service)
    {
        ApiService = service;
    }

    public async Task LoadTasksAsync()
    {
        IsLoading = true;

        var userId = Preferences.Default.Get("user_id", 0);
        
        var result = await ApiService.GetTasksForThemeAsync(ThemeId, userId);

        if (result is null)
        {
            IsLoading = false;
            return;
        }
        
        Model.Clear();

        foreach (var task in result)
        {
            task.BuildVariants();
            Model.Add(task);
        }
        
        IsLoading = false;
    }
}