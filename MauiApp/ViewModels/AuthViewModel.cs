using System.Diagnostics;
using System.Security.Claims;
using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class AuthViewModel : ViewModelBase<AuthModel>
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
    public string? Username
    {
        get => Model.Username;

        set
        {
            if (Username != value)
            {
                Model.Username = value;
                OnPropertyChanged();
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string? Password
    {
        get => Model.Password;
        set
        {
            if (Password != value)
            {
                Model.Password = value;
                OnPropertyChanged();
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }
    }

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
        return !string.IsNullOrWhiteSpace(authModel.Username) && !string.IsNullOrWhiteSpace(authModel.Password);
    }

    private async void ExecuteLogin(object obj)
    {
        if (obj is not AuthModel authModel) return;
            
        if (string.IsNullOrWhiteSpace(authModel.Username) || string.IsNullOrWhiteSpace(authModel.Password))
        {
            return;
        }

        if (await AuthenticateUser(authModel))
        {
            ErrorMessage = null;
            
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current != null) Application.Current.MainPage = new AppShell();

                await Shell.Current.GoToAsync($"//ThemesView");
            });
        }
        else
        {
            ErrorMessage = "Ошибка при входе";
        }
    }
        
    private async Task<bool> AuthenticateUser(AuthModel authModel)
    {
        var token = await _apiService.Login(authModel);

        if (token == null) return false;
        
        try
        {
            await SecureStorage.Default.SetAsync("auth_token", token);
            
            var claims = TokenParseService.DecodeClaims(token);
            
            if (claims.TryGetValue("nameid", out var userId))
            { 
                Preferences.Default.Set("user_id", Convert.ToInt32(userId));
            }

            if (claims.TryGetValue("unique_name", out var username))
            {
                Preferences.Default.Set("username", username);
            }

            if (claims.TryGetValue("exp", out var exp))
            {
                Preferences.Default.Set("exp", exp);
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }
    }
}