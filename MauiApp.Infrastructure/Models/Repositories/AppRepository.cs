using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Services;

namespace MauiApp.Infrastructure.Models.Repositories;

public class AppRepository(ApiService apiService, LocalDataService localDataService)
{
    public async Task<string?> Login(AuthModel authModel)
    {
        try
        {
            return await apiService.Login(authModel);
        }
        catch (HttpRequestException)
        {
            return await localDataService.Login(authModel);
        }
    }

    public async Task<bool> Register(RegisterModel regModel)
    {
        try
        {
            return await apiService.RegisterUser(regModel);
        }
        catch (HttpRequestException)
        {
            return await localDataService.Register(regModel);
        }
    }

    public async Task<List<Theme>?> GetThemesAsync()
    {
        try
        {
            return await apiService.GetThemesAsync();
        }
        catch (HttpRequestException)
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
