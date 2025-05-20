using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class ThemesViewModel : ViewModelBase<List<Theme>>
{
    public ThemesViewModel(ApiService service)
    {
        _apiService = service;
        
        LoadThemes();
    }
    
    private void LoadThemes()
    {
        var result = _apiService.GetThemesAsync().Result;
        
        Model = result ?? new List<Theme>();
    }

}