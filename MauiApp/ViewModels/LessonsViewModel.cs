using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class LessonsViewModel : ViewModelBase<List<Lesson>>
{
    public int ThemeId { get; set; }
    
    public bool HasItems => Model is { Count: > 0 };
    public bool IsEmpty => Model is not { Count: not 0 };

    public LessonsViewModel(ApiService service)
    {
        _apiService = service;
    }

    public async void LoadLessonsAsync()
    {
        var result = await _apiService.GetLessonsForThemeAsync(ThemeId);
        
        Model = result ?? new List<Lesson>();
        
        OnPropertyChanged(nameof(Model));
    }
}