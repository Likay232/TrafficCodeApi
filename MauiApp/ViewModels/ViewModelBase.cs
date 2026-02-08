using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp.Infrastructure.Models.Repositories;

namespace MauiApp.ViewModels;

public class ViewModelBase<T> : INotifyPropertyChanged where T : class, new()
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public T Model { get; set; } = new ();
    
    protected AppRepository AppRepository = null!;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string property = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
    
    protected bool SetProperty<TPropertyType>(ref TPropertyType backingStore, TPropertyType value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<TPropertyType>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    public ICommand BackCommand => new Command(async void () =>
    {
        await Shell.Current.GoToAsync("..", animate: false);
    });
}