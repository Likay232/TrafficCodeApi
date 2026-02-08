using MauiApp.Infrastructure.Models.Repositories;
using MauiApp.Infrastructure.Models.Сomponents;
using MauiApp.Infrastructure.Services;
using MauiApp.Services;
using MauiApp.ViewModels;
using MauiApp.Views;
using Microsoft.EntityFrameworkCore;

namespace MauiApp.ApplicationExtension;

public static class RunExtension
{
    public static Microsoft.Maui.Hosting.MauiApp MigrateLocalDatabase(this Microsoft.Maui.Hosting.MauiApp app)
    {
        using var scope = app.Services.CreateScope();

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }

        return app;
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<LocalDataService>();
        builder.Services.AddSingleton<SharedObjectStorageService>();
        builder.Services.AddTransient<AppRepository>();
        builder.Services.AddTransient<DataComponent>();
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddTransient<ServerPingService>();

        
        return builder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<LessonsViewModel>();
        builder.Services.AddTransient<TasksViewModel>();
        builder.Services.AddTransient<TestViewModel>();
        builder.Services.AddTransient<ThemesViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        
        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<AuthView>();
        builder.Services.AddTransient<LessonsView>();
        builder.Services.AddTransient<TasksView>();
        builder.Services.AddTransient<TestView>();
        builder.Services.AddTransient<ThemesView>();
        builder.Services.AddTransient<RegisterView>();
        builder.Services.AddTransient<ProfileView>();
        builder.Services.AddTransient<PracticeView>();
        
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