using System.Diagnostics;
using System.Windows.Input;
using MauiApp.Infrastructure.Models.Commands;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Repositories;
using MauiApp.Infrastructure.Services;

namespace MauiApp.ViewModels;

public class AuthViewModel : ViewModelBase<AuthModel>
{
    private string? _errorMessage;

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage == value) return;

            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string? Username
    {
        get => Model.Username;

        set
        {
            if (Username == value) return;

            Model.Username = value;
            OnPropertyChanged();
            ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
        }
    }

    public string? Password
    {
        get => Model.Password;
        set
        {
            if (Password == value) return;

            Model.Password = value;
            OnPropertyChanged();
            ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand LoginCommand { get; set; }
    private ExchangeDataService ExchangeDataService { get; set; }

    public AuthViewModel(AppRepository repository, ExchangeDataService exchangeDataService)
    {
        AppRepository = repository;
        ExchangeDataService = exchangeDataService;

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

            MainThread.BeginInvokeOnMainThread(async void () =>
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
        var tokens = await AppRepository.Login(authModel);

        if (tokens == null) return false;
        if (string.IsNullOrEmpty(tokens.AccessToken) && string.IsNullOrEmpty(tokens.RefreshToken))
        {
            if (Preferences.Get("username", null) is null)
                Preferences.Default.Set("username", "Нет сети");

            return true;
        }

        try
        {
            await LocalDataService.SetUserInfo(tokens.AccessToken, tokens.RefreshToken);
            
            _ = ApiService.RegisterDevice();
            _ = ExchangeDataService.ExchangeDataWithServer();

            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }
    }
}