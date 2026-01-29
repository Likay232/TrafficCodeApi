using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Services;
using MauiApp.Services;

namespace MauiApp.Repositories;

public class AppRepository(ApiService apiService, LocalDbService localDbService)
{
    public async Task<string?> Login(AuthModel authModel)
    {
        string? token;
        
        try
        {
            token = await apiService.Login(authModel);
        }
        catch (HttpRequestException)
        {
            token = await localDbService.Login(authModel);
        }

        return token;
    }

    public async Task<bool> Register(RegisterModel regModel)
    {
        try
        {
            return await apiService.RegisterUser(regModel);
        }
        catch (HttpRequestException)
        {
            return await localDbService.Register(regModel);
        }
    }
}