using MauiApp.Infrastructure.Models.Repositories;
using MauiApp.Infrastructure.Services;
using MauiApp.Services;
using MauiApp.Views;

namespace MauiApp;

public partial class App
{
    private ServerPingBackgroundService PingBackgroundService { get; set; }
    public App(AuthView authView, ServerPingBackgroundService backgroundService)
    {
        PingBackgroundService = backgroundService;
        
        InitializeComponent();

        Current!.UserAppTheme = AppTheme.Light;
        
        var accessToken = SecureStorage.GetAsync("auth_token").Result;
        
        if ((string.IsNullOrEmpty(accessToken)) ||
            TokenService.IsExpired(accessToken))
        {
            MainPage = authView;
        }
        else
        {
            MainPage = new AppShell();
        }
    }

    protected override async void OnStart()
    {
        base.OnStart();

        PingBackgroundService.Start();
        
        // var accessToken = SecureStorage.GetAsync("auth_token").Result;
        // var refreshToken = SecureStorage.GetAsync("refresh_token").Result;
        //
        // if (TokenService.IsExpired(accessToken) || TokenService.ShouldRefresh(accessToken))
        // {
        //     if (!ApiService.PingServer().Result)
        //         MainPage = new AppShell();
        //     
        //     var tokens = ApiService.RefreshToken(refreshToken).Result;
        //
        //     if (tokens is null)
        //     {
        //         SecureStorage.Remove("auth_token");
        //         SecureStorage.Remove("refresh_token");
        //         Preferences.Clear();
        //
        //         await Shell.Current.GoToAsync("//AuthView");
        //     }
        // }

    }

    protected override void OnSleep()
    {
        base.OnSleep();
        
        PingBackgroundService.Stop();
    }
}