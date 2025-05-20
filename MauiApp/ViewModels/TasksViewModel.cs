using MauiApp.Services;

namespace MauiApp.ViewModels;

[QueryProperty(nameof(ThemeId), "themeId")]
public class TasksViewModel : ViewModelBase<List<Task>>
{
    private int _themeId;
    public int ThemeId
    {
        get => _themeId;
        set
        {
            _themeId = value;
            LoadTasks();
        }
    }

    public TasksViewModel(ApiService service)
    {
        _apiService = service;
    }

    private void LoadTasks()
    {
        var result = _apiService.GetTasksForThemeAsync(_themeId).Result;
        Model = result ?? new List<Task>();
    }
}