using System.Windows.Input;

namespace MauiApp.Commands;

public class BackButtonCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("..", animate: false);
        }
    }
}