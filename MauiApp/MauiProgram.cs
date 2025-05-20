using MauiApp.Pages;
using MauiApp.Services;
using MauiApp.ViewModels;
using MauiApp.Views;

namespace MauiApp;

using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<AuthView>();
        builder.Services.AddTransient<ThemesViewModel>();
        builder.Services.AddTransient<ThemesView>();
        
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}