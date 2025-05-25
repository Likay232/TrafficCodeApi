using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TestsViewModel : ViewModelBase<List<Test>>
{
    public TestsViewModel(ApiService service)
    {
        _apiService = service;
    }

    public async void LoadTests()
    {
        var result = await _apiService.GetTestsAsync();
        Model = result ?? new List<Test>();
        OnPropertyChanged(nameof(Model));
    }

}