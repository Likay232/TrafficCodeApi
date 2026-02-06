using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MauiApp.Infrastructure.Services;

public class ApiService
{
    private static readonly string ServerAddress = $"http://192.168.1.124:5000/";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private HttpClient GetClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var token = SecureStorage.GetAsync("auth_token").Result;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.BaseAddress = new Uri(ServerAddress);
        return client;
    }

    public async Task<string?> Login(AuthModel authModel)
    {
        var response = await GetClient().PostAsJsonAsync("/Auth/Login", authModel, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        
        var token = await response.Content.ReadFromJsonAsync<string>(_jsonSerializerOptions);

        return token;
    }
    

    public async Task<List<Theme>?> GetThemesAsync()
    {
        try
        {
            var response = await GetClient().GetAsync("Client/GetThemes");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Theme>>(json, _jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[GetThemesAsync] Exception: {e}");
            return null;
        }
    }


    public async Task<List<TaskForTest>?> GetTasksForThemeAsync(int themeId, int userId)
    {
        try
        {
            var response = await GetClient().PostAsJsonAsync($"/Client/GetTasksForTheme", new { themeId, userId }, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<TaskForTest>>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GetTasksForThemeAsync] {e.Message}");
            return null;
        }
    }

    public async Task<List<Lesson>?> GetLessonsForThemeAsync(int themeId)
    {
        try
        {
            var response = await GetClient().GetAsync($"/Client/GetLessonsForTheme?themeId={themeId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Lesson>>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GetLessonsForThemeAsync] {e.Message}");
            return null;
        }
    }

    public async Task<List<Test>?> GetTestsAsync()
    {
        try
        {
            var response = await GetClient().GetAsync("/Client/GetTests");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Test>>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GetTestsAsync] {e.Message}");
            return null;
        }
    }

    public async Task<List<TaskForTest>?> GetTestAsync(int testId)
    {
        try
        {
            var response = await GetClient().GetAsync($"/Client/GetTest?testId={testId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<TaskForTest>>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GetTestAsync] {e.Message}");
            return null;
        }
    }
    
    public async Task<TaskForTest?> GetRandomTask()
    {
        try
        {
            var response = await GetClient().GetAsync($"/Client/GetRandomTask");
            response.EnsureSuccessStatusCode();
            
            Debug.WriteLine(response);
            
            return await response.Content.ReadFromJsonAsync<TaskForTest>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[GetRandomTask] {e.Message}");
            return null;
        }
    }
    
    public async Task<bool> RegisterUser(RegisterModel request)
    {
        var response = await GetClient().PostAsJsonAsync("/Auth/Register", request, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<bool>(_jsonSerializerOptions);
    }
    
    public async Task<Test?> GenerateTest(TestTypes testType, int userId, int? themeId = null)
    {
        try
        {
            var response = await GetClient()
                .GetAsync($"/Client/GenerateTest?testType={testType}&themeId={themeId}&userId={userId}");
            response.EnsureSuccessStatusCode();

            var temp = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Test>(temp);
            
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public async Task<ProfileInfo?> GetProfileInfo(int userId)
    {
        try
        {
            var response = await GetClient().GetAsync($"/Client/GetProfileInfo?userId={userId}");
            response.EnsureSuccessStatusCode();
    
            return await response.Content.ReadFromJsonAsync<ProfileInfo>(_jsonSerializerOptions);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<bool> SaveAnswer(int userId, int taskId, bool isCorrect)
    {
        try
        {
            var response = 
                await GetClient().PostAsync($"/Client/SaveAnswer?userId={userId}&taskId={taskId}&isCorrect={isCorrect}", null);
            response.EnsureSuccessStatusCode();
    
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<bool> SaveAnswers(int userId, List<UserAnswer> answers)
    {
        try
        {
            var payload = new
            {
                userId,
                answers
            };

            var response = await GetClient()
                .PostAsJsonAsync("/Client/SaveAnswers", payload, _jsonSerializerOptions);

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static string? GetAbsoluteFilePath(string? filePath)
    {
        return filePath is null ? null : $"{ServerAddress}{filePath}";
    }

    public async Task<byte[]?> GetFileBytes(string fileName)
    {
        try
        {
            var response = await GetClient().GetAsync($"/Client/GetFileBytes?fileName={fileName}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<byte[]?>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[GetFileBytes] {e.Message}");
            return null;
        }
    }
}