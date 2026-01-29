using System.ComponentModel;

namespace MauiApp.Infrastructure.Models.DTO;

public class ThemeTaskCount : INotifyPropertyChanged
{
    public int ThemeId { get; set; }
    
    public string ThemeTitle { get; set; } = string.Empty;

    private int _taskCount;
    public int TaskCount
    {
        get => _taskCount;
        set
        {
            if (_taskCount != value)
            {
                _taskCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskCount)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
