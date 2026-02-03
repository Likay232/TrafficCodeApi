using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp.Infrastructure.Models.DTO;

public class TaskNavigationItem : INotifyPropertyChanged
{

    private int _index;
    public int Index {
        get => _index;
        set => SetProperty(ref _index, value);
    }

    private TaskForTest _task = new();

    public TaskForTest Task
    {
        get => _task;
        set  => SetProperty(ref _task, value);
    }
    
    private bool _isCurrent;
    public bool IsCurrent
    {
        get => _isCurrent;
        set => SetProperty(ref _isCurrent, value);
    }

    private bool _isAnswered;
    public bool IsAnswered
    {
        get => _isAnswered;
        set => SetProperty(ref _isAnswered, value);
    }

    private bool? _isCorrect;
    public bool? IsCorrect
    {
        get => _isCorrect;
        set => SetProperty(ref _isCorrect, value);
    }

    public int DisplayIndex => Index + 1;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private bool SetProperty<T>(ref T storage, T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

}