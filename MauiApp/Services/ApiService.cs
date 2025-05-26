using System.Net.Http.Json;
using System.Text.Json;
using MauiApp.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace MauiApp.Services;

public class ApiService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private HttpClient GetClient()
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var token = SecureStorage.GetAsync("auth_token").Result;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.BaseAddress = new Uri("http://192.168.1.123:5000/");
        return client;
    }

    public async Task<string?> Login(AuthModel authModel)
    {
        var response = await GetClient().PostAsJsonAsync("/Auth/LoginClient", authModel, _jsonSerializerOptions);
        string? token = null;
        
        try
        {
            response.EnsureSuccessStatusCode();
            token = await response.Content.ReadFromJsonAsync<string>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return null;
        }

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

    public async Task<CheckedTest?> CheckTestAsync(TestForCheck request)
    {
        try
        {
            var response = await GetClient().PostAsJsonAsync("/Client/CheckTest", request, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CheckedTest>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[CheckTestAsync] {e.Message}");
            return null;
        }
    }

    public async Task<TaskForTest?> GetTaskById(int taskId)
    {
        try
        {
            var response = await GetClient().GetAsync($"/Client/GetTaskById?taskId={taskId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TaskForTest>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}