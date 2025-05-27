using System.Text.Json;
using MauiApp.Models;
using MauiApp.Services;
using MauiApp.ViewModels;
using MauiApp.Views;

namespace MauiApp;

#if ANDROID
    using Android.Content;
#endif

public partial class App : Application
{
    public App(AuthView authView)
    {
        InitializeComponent();

        var token = SecureStorage.GetAsync("auth_token").Result;
        
        if (string.IsNullOrEmpty(token) || TokenParseService.IsExpired())
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

#if ANDROID
        
        var task = await new ApiService().GetRandomTask();
        if (task != null)
        {
            var serializedTask = JsonSerializer.Serialize(task);
            var context = Android.App.Application.Context;
            var intent = new Intent(context, typeof(RandomTaskWidget));
            intent.SetAction("com.myapp.ACTION_UPDATE_WIDGET");
            intent.PutExtra("task", serializedTask);
            context.SendBroadcast(intent);
        }
#endif
    }
}