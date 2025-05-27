using System.Collections.ObjectModel;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class CheckedTestViewModel : ViewModelBase<CheckedTest>
{
    public CheckedTestViewModel(TestResultScore testResultStore)
    {
        Model = testResultStore.CurrentResult ?? new CheckedTest();
        
        OnPropertyChanged(nameof(Model));
    }
}