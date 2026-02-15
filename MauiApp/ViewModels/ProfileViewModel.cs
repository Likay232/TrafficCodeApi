using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Repositories;

namespace MauiApp.ViewModels;

public class ProfileViewModel : ViewModelBase<ProfileInfo>
{
    private bool _noInfo;

    public bool NoInfo
    {
        get => _noInfo;
        set => SetProperty(ref _noInfo, value);
    }
    public ProfileViewModel(AppRepository repository)
    {
        AppRepository = repository;
    }
    
    public async Task LoadProfileInfo()
    {
        var userId = Preferences.Default.Get("user_id", 0);

        var result = await AppRepository.GetProfileInfo(userId);
        
        NoInfo = result is null;
        
        if (NoInfo) return;
        
        Model = result ?? new ProfileInfo();
        
        OnPropertyChanged(nameof(Model));
    }

}