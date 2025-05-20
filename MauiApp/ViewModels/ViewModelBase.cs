using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class ViewModelBase<T> : INotifyPropertyChanged where T : class
{
    public T Model { get; set; }
    
    protected ApiService _apiService;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string property = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}