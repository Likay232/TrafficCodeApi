using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TestsViewModel : ViewModelBase<List<Test>>
{
    public TestsViewModel(ApiService service)
    {
        _apiService = service;
        LoadTests();
    }

    private void LoadTests()
    {
        var result = _apiService.GetTestsAsync().Result;
        Model = result ?? new List<Test>();
    }

}