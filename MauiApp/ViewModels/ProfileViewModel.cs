using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Repositories;

namespace MauiApp.ViewModels;

public class ProfileViewModel : ViewModelBase<ProfileInfo>
{
    public ProfileViewModel(AppRepository repository)
    {
        AppRepository = repository;
    }
    
    public async Task LoadProfileInfo()
    {
        var userId = Preferences.Default.Get("user_id", 0);

        var result = await AppRepository.GetProfileInfo(userId);
        
        Model = result ?? new ProfileInfo();
        
        OnPropertyChanged(nameof(Model));
    }

}