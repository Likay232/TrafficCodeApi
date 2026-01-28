using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiApp.Repositories;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class ViewModelBase<T> : INotifyPropertyChanged where T : class
{
    public T Model { get; set; }
    
    protected ApiService _apiService;
    protected AppRepository AppRepository;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string property = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}