using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp.Infrastructure.Models.DTO;

public class AnswerVariant : INotifyPropertyChanged
{
    public string Text { get; set; } = string.Empty;
    
    private bool _isCorrect;
    public bool IsCorrect
    {
        get => _isCorrect;
        set
        {
            if (_isCorrect == value) return;
            _isCorrect = value;
            OnPropertyChanged();
        }
    }
    
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(name));

}