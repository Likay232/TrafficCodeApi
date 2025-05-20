using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

[QueryProperty(nameof(ThemeId), "themeId")]
public class LessonsViewModel : ViewModelBase<List<Lesson>>
{
    private int _themeId;

    public int ThemeId
    {
        get => _themeId;
        set
        {
            _themeId = value;
            LoadLessons();
        }
    }

    public LessonsViewModel(ApiService service)
    {
        _apiService = service;
    }

    private void LoadLessons()
    {
        var result = _apiService.GetLessonsForThemeAsync(_themeId).Result;
        
        Model = result ?? new List<Lesson>();
    }
}