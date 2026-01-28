using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Services;
using MauiApp.Views;

namespace MauiApp.ViewModels;

public class GenerateTestViewModel : ViewModelBase<GenerateTest>
{
    public ObservableCollection<ThemeTaskCount> ThemeTaskCounts { get; set; } = new();
    public ICommand GenerateTestCommand { get; }

    private readonly SharedObjectStorageService _storage;

    public GenerateTestViewModel(ApiService service, SharedObjectStorageService storage)
    {
        _apiService = service;
        _storage = storage;
        
        Model = new GenerateTest();
        
        GenerateTestCommand = new RelayCommand(ExecuteGenerateTest, CanExecuteGenerateTest);
    }

    public async void LoadThemesAsync()
    {
        var result = await _apiService.GetThemesAsync();
        var themes = result ?? new List<Theme>();

        ThemeTaskCounts.Clear();
        foreach (var theme in themes)
        {
            ThemeTaskCounts.Add(new ThemeTaskCount
            {
                ThemeId = theme.Id,
                ThemeTitle = theme.Title,
                TaskCount = 0
            });
        }
    }

    private bool CanExecuteGenerateTest(object obj)
    {
        return true;
    }

    private async void ExecuteGenerateTest(object obj)
    {
        if (!ThemeTaskCounts.Any(t => t.TaskCount > 0)) return;
        
        var generateRequest = new GenerateTest
        {
            DesiredTasksAmount = ThemeTaskCounts
                .Where(t => t.TaskCount > 0)
                .ToDictionary(t => t.ThemeId, t => t.TaskCount),
            UserId = Preferences.Default.Get("user_id", 0)
        };

        var generatedTasks = await _apiService.GenerateTest(generateRequest);

        if (generatedTasks is { Count: > 0 })
        {
            _storage.CurrentTest = generatedTasks;

            await Shell.Current.GoToAsync(nameof(TestView));
        }
    }
}
