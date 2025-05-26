using System.Collections.ObjectModel;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class CheckedTestViewModel(ApiService service) : ViewModelBase<CheckedTest>
{
    public void UpdateModel()
    {
        OnPropertyChanged(nameof(Model));
    }
}