using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Services;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TestsViewModel : ViewModelBase<List<Test>>
{
    private new readonly SharedObjectStorageService _storage;

    public TestsViewModel(ApiService service, SharedObjectStorageService storage)
    {
        ApiService = service;
        _storage = storage;
    }

    public async void LoadTests()
    {
        var result = await ApiService.GetTestsAsync();
        Model = result ?? new List<Test>();
        OnPropertyChanged(nameof(Model));
    }

    public async Task<bool> GetTestById(int testId)
    {
        var test = await ApiService.GetTestAsync(testId);

        if (test == null) return false;
    
        _storage.CurrentTest = test;
        return true;
    }
}