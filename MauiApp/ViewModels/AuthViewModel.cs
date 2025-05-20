using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class AuthViewModel : ViewModelBase<AuthModel>
{
    public string? ErrorMessage { get; set; }
    
    public ICommand LoginCommand { get; set; }

    public AuthViewModel(ApiService service)
    {
        _apiService = service;
        
        Model = new AuthModel();
        LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
    }
    
    private bool CanExecuteLogin(object obj)
    {
        if (obj is not AuthModel authModel) return false;
        return authModel is { Username: not null, Password: not null };
    }

    private void ExecuteLogin(object obj)
    {
        if (obj is not AuthModel authModel) return;
            
        if (string.IsNullOrWhiteSpace(authModel.Username) || string.IsNullOrWhiteSpace(authModel.Password))
        {
            return;
        }

        if (AuthenticateUser(authModel.Username, authModel.Password))
        {
            
        }
        else
        {
            ErrorMessage = "Ошибка при входе";
        }
    }
        
    private bool AuthenticateUser(string username, string password)
    {
        return true;
    }

}