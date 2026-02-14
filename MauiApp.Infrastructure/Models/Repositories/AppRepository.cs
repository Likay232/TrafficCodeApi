using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Models.Requests;
using MauiApp.Infrastructure.Models.Responses;
using MauiApp.Infrastructure.Services;

namespace MauiApp.Infrastructure.Models.Repositories;

public class AppRepository(ApiService apiService, LocalDataService localDataService)
{
    public async Task<Login?> Login(AuthModel authModel)
    {
        try
        {
            var token = await apiService.Login(authModel);
            
            return token;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode is null)
            {
                var response = new Login();
                
                var refreshToken = await localDataService.Login();

                if (refreshToken is null) return response;
            
                response.RefreshToken = refreshToken;
                
                return response;
            }

            return null;
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
            return await ApiService.PostData<List<TaskForTest>, GetTasks>("/Client/GetTasksForTheme", new GetTasks
            {
                ThemeId = themeId, 
                UserId = userId
            });
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
            return await ApiService.GetData<List<Lesson>>($"/Client/GetLessonsForTheme?themeId={themeId}");
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
            return await ApiService.GetData<Test>($"/Client/GenerateTest?testType={type}&themeId={themeId}&userId={userId}");
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
            return await ApiService.GetData<ProfileInfo>($"/Client/GetProfileInfo?userId={userId}");
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
            return await ApiService.PostData<bool, object?>($"/Client/SaveAnswer?userId={userId}&taskId={taskId}&isCorrect={isCorrect}");
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
            return await ApiService.PostData<bool, SaveAnswers>("/Client/SaveAnswers", new SaveAnswers()
            {
                UserId = userId,
                UserAnswers = answers
            });

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
