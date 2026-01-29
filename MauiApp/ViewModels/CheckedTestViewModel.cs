using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Services;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class CheckedTestViewModel : ViewModelBase<CheckedTest>
{
    public CheckedTestViewModel(SharedObjectStorageService testResultStore)
    {
        Model = testResultStore.CurrentResult ?? new CheckedTest();
        
        OnPropertyChanged(nameof(Model));
    }
}