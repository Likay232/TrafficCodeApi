using MauiApp.Repositories;
using MauiApp.Services;
using MauiApp.ViewModels;
using MauiApp.Views;

namespace MauiApp.ApplicationExtension;

public static class RunExtension
{
    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<LocalDbService>();
        builder.Services.AddSingleton<SharedObjectStorageService>();
        builder.Services.AddTransient<AppRepository>();

        
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
        builder.Services.AddTransient<TaskViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<GenerateTestViewModel>();
        
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
        builder.Services.AddTransient<TaskView>();
        builder.Services.AddTransient<RegisterView>();
        builder.Services.AddTransient<GenerateTestView>();
        
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