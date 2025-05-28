using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TestsViewModel : ViewModelBase<List<Test>>
{
    private new readonly SharedObjectStorageService _storage;

    public TestsViewModel(ApiService service, SharedObjectStorageService storage)
    {
        _apiService = service;
        _storage = storage;
    }

    public async void LoadTests()
    {
        var result = await _apiService.GetTestsAsync();
        Model = result ?? new List<Test>();
        OnPropertyChanged(nameof(Model));
    }

    public async Task<bool> GetTestById(int testId)
    {
        var test = await _apiService.GetTestAsync(testId);

        if (test == null) return false;
    
        _storage.CurrentTest = test;
        return true;
    }
}