using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Repositories;

namespace MauiApp.ViewModels;

public class ThemesViewModel : ViewModelBase<List<Theme>>
{
    public ThemesViewModel(AppRepository repository)
    {
        AppRepository = repository;
    }
    
    public async void LoadThemesAsync()
    {
        var result = await AppRepository.GetThemesAsync();

        Model = result ?? new List<Theme>();

        Model = Model.OrderBy(t => t.Title).ToList();
        
        OnPropertyChanged(nameof(Model));
    }

}