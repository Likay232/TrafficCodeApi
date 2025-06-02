using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class ThemesViewModel : ViewModelBase<List<Theme>>
{
    public ThemesViewModel(ApiService service)
    {
        _apiService = service;
    }
    
    public async void LoadThemesAsync()
    {
        var result = await _apiService.GetThemesAsync();

        Model = result ?? new List<Theme>();

        Model = Model.OrderBy(t => t.Title).ToList();
        
        OnPropertyChanged(nameof(Model));
    }

}