using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Services;

namespace MauiApp.ViewModels;

public class ProfileViewModel : ViewModelBase<ProfileInfo>
{
    public ProfileViewModel(ApiService service)
    {
        ApiService = service;
    }

    public async Task LoadProfileInfo()
    {
        var userId = Preferences.Default.Get("user_id", 0);

        var result = await ApiService.GetProfileInfo(userId);
        
        Model = result ?? new ProfileInfo();
        
        OnPropertyChanged(nameof(Model));
    }

}