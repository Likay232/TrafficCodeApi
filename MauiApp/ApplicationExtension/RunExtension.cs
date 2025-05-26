using MauiApp.Services;
using MauiApp.ViewModels;
using MauiApp.Views;

namespace MauiApp.ApplicationExtension;

public static class RunExtension
{
    public static MauiAppBuilder RegisterSerivces(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ApiService>();
        
        return builder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<LessonsViewModel>();
        builder.Services.AddTransient<TasksViewModel>();
        builder.Services.AddTransient<TestsViewModel>();
        builder.Services.AddTransient<TestViewModel>();
        builder.Services.AddTransient<ThemesViewModel>();
        builder.Services.AddTransient<CheckedTestViewModel>();
        
        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<AuthView>();
        builder.Services.AddTransient<LessonsView>();
        builder.Services.AddTransient<TasksView>();
        builder.Services.AddTransient<TestsView>();
        builder.Services.AddTransient<TestView>();
        builder.Services.AddTransient<ThemesView>();
        builder.Services.AddTransient<CheckedTestView>();
        
        return builder;
    }

    public static MauiAppBuilder RegisterDeviceToken(this MauiAppBuilder builder)
    {
        /*CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
        {
            string token = p.Token;

            var service = new ApiService();
            
            
        };
        */
        return builder;
    }
}