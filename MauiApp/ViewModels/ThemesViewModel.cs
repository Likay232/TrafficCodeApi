using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Services;

namespace MauiApp.ViewModels;

public class ThemesViewModel : ViewModelBase<List<Theme>>
{
    public ThemesViewModel(ApiService service)
    {
        ApiService = service;
    }
    
    public async void LoadThemesAsync()
    {
        var result = await AppRepository.GetThemesAsync();

        Model = result ?? new List<Theme>();

        Model = Model.OrderBy(t => t.Title).ToList();
        
        OnPropertyChanged(nameof(Model));
    }

}