using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using MauiApp.Models;
using Task = System.Threading.Tasks.Task;

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
        return client;
    }

    public async Task<List<Theme>?> GetThemesAsync()
    {
        try
        {
            var response = await GetClient().GetAsync("/Client/GetUsers");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Theme>>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GetThemesAsync] {e.Message}");
            return null;
        }
    }

    public async Task<List<Task>?> GetTasksForThemeAsync(int themeId)
    {
        try
        {
            var response = await GetClient().GetAsync($"/Client/GetTasksForTheme?themeId={themeId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Task>>(_jsonSerializerOptions);
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

    public async Task<List<TaskForTest>?> CheckTestAsync(TestForCheck request)
    {
        try
        {
            var response = await GetClient().PostAsJsonAsync("/Client/CheckTest", request, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<TaskForTest>>(_jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[CheckTestAsync] {e.Message}");
            return null;
        }
    }
}
