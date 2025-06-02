using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class ChangePassViewModel : ViewModelBase<ChangePassRequest>
{
    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public string? OldPassword
    {
        get => Model.OldPassword;
        set
        {
            if (OldPassword != value)
            {
                Model.OldPassword = value ?? "";
                OnPropertyChanged();
                ((RelayCommand)ChangePassCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string? NewPassword
    {
        get => Model.NewPassword;
        set
        {
            if (NewPassword != value)
            {
                Model.NewPassword = value ?? "";
                OnPropertyChanged();
                ((RelayCommand)ChangePassCommand).RaiseCanExecuteChanged();
            }
        }
    }
    
    public ICommand ChangePassCommand { get; }
    
    public ChangePassViewModel(ApiService service)
    {
        _apiService = service;

        Model = new ChangePassRequest
        {
            UserId = Preferences.Default.Get("user_id", 0)
        };

        ChangePassCommand = new RelayCommand(ExecuteChangePass, CanExecuteChangePass);
    }
    
    private bool CanExecuteChangePass(object obj)
    {
        if (obj is not ChangePassRequest request) return false;
        return !string.IsNullOrWhiteSpace(request.OldPassword) 
               && !string.IsNullOrWhiteSpace(request.NewPassword)
               && !request.OldPassword.Equals(request.NewPassword)
               && !request.NewPassword.Contains(" ");
    }

    private async void ExecuteChangePass(object obj)
    {
        if (obj is not ChangePassRequest request) return;
            
        if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return;
        }

        if (await _apiService.ChangePassword(request))
        {
            ErrorMessage = null;
            
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"//ThemesView");
            });
        }
        else
        {
            ErrorMessage = "Неверный пароль.";
        }

    }
}