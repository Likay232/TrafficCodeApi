using System.Diagnostics;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TasksViewModel : ViewModelBase<List<TaskForTest>>
{
    public int ThemeId { get; set; }

    public TasksViewModel(ApiService service)
    {
        _apiService = service;
    }

    public async void LoadTasksAsync()
    {
        var userId = Convert.ToInt32(Preferences.Default.Get<string>("user_id", "0"));
        
        var result = await _apiService.GetTasksForThemeAsync(ThemeId, userId);
        Model = result ?? new List<TaskForTest>();
        OnPropertyChanged(nameof(Model));
    }
}