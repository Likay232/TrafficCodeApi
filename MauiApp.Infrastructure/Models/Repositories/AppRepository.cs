using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Services;

namespace MauiApp.Infrastructure.Models.Repositories;

public class AppRepository(ApiService apiService, LocalDataService localDataService)
{
    public async Task<(string AccessToken, string RefreshToken)?> Login(AuthModel authModel)
    {
        try
        {
            var token = await apiService.Login(authModel);
            
            return token;
        }
        catch (HttpRequestException)
        {
            var refreshToken = await localDataService.Login();

            if (refreshToken is null) return ("", "");
            
            return ("", refreshToken);
        }
    }

    public async Task<bool> Register(RegisterModel regModel)
    {
        try
        {
            var registered =  await apiService.RegisterUser(regModel);
            
            return registered;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public async Task<List<Theme>?> GetThemesAsync()
    {
        try
        {
            return await ApiService.GetData<List<Theme>>("Client/GetThemes");
        }
        catch (Exception)
        {
            return await localDataService.GetThemesAsync();
        }
    }

    public async Task<List<TaskForTest>?> GetTasksForTheme(int themeId, int userId)
    {
        try
        {
            return await apiService.GetTasksForThemeAsync(themeId, userId);
        }
        catch (HttpRequestException)
        {
            return await localDataService.GetTasksForTheme(userId, themeId);
        }
    }

    public async Task<List<Lesson>?> GetLessonsForTheme(int themeId)
    {
        try
        {
            return await apiService.GetLessonsForThemeAsync(themeId);
        }
        catch (HttpRequestException)
        {
            return await localDataService.GetLessonsForTheme(themeId);
        }
    }

    public async Task<Test?> GenerateTest(TestTypes type, int userId, int? themeId = null)
    {
        try
        {
            return await apiService.GenerateTest(type, userId, themeId);
        }
        catch (HttpRequestException)
        {
            return await localDataService.GenerateTest(type, userId, themeId);
        }
    }

    public async Task<ProfileInfo?> GetProfileInfo(int userId)
    {
        try
        {
            return await apiService.GetProfileInfo(userId);
        }
        catch (HttpRequestException)
        {
            return await localDataService.GetProfileInfo(userId);
        }
    }

    public async Task<bool> SaveAnswer(int userId, int taskId, bool isCorrect)
    {
        try
        {
            return await apiService.SaveAnswer(userId, taskId, isCorrect);
        }
        catch (HttpRequestException)
        {
            await localDataService.SaveAnswer(userId, taskId, isCorrect);
            return true;
        }
    }
    
    public async Task<bool> SaveAnswers(int userId, List<UserAnswer> answers)
    {
        try
        {
            return await apiService.SaveAnswers(userId, answers);
        }
        catch (HttpRequestException)
        {
            await localDataService.SaveAnswers(userId, answers);
            return true;
        }
    }
    
    public static string? GetFileAbsolutePath(string? filePath)
    {
        try
        {
            return ApiService.GetAbsoluteFilePath(filePath);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

}
