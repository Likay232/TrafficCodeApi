using System.Diagnostics;
using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TasksViewModel : ViewModelBase<List<TaskForTest>>
{
    public int ThemeId { get; set; }

    public ICommand DownloadFileCommand { get; set; }

    public TasksViewModel(ApiService service)
    {
        _apiService = service;

        DownloadFileCommand = new DownloadFileCommand(service);
    }

    public async void LoadTasksAsync()
    {
        var userId = Preferences.Default.Get("user_id", 0);
        
        var result = await _apiService.GetTasksForThemeAsync(ThemeId, userId);
        Model = result ?? new List<TaskForTest>();
        
        Model = Model.OrderBy(t => t.DifficultyLevel).ToList();
        
        OnPropertyChanged(nameof(Model));
    }
}