using MauiApp.Infrastructure.Models.Repositories;
using MauiApp.Infrastructure.Services;
using MauiApp.Services;
using MauiApp.Views;

namespace MauiApp;

public partial class App
{
    private ServerPingService PingService { get; set; }
    public App(AuthView authView, ServerPingService service)
    {
        PingService = service;
        
        InitializeComponent();

        Current!.UserAppTheme = AppTheme.Light;
        
        var token = SecureStorage.GetAsync("auth_token").Result;
        var username = SecureStorage.GetAsync("username").Result;
        
        if ((string.IsNullOrEmpty(token) && string.IsNullOrEmpty(username)) ||
            TokenParseService.IsExpired())
        {
            MainPage = authView;
        }
        else
        {
            MainPage = new AppShell();
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        PingService.Start();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        
        PingService.Stop();
    }
}