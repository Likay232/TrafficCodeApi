using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Models.Requests;
using MauiApp.Infrastructure.Models.Responses;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MauiApp.Infrastructure.Services;

public class ApiService
{
    private const string ServerAddress = "http://192.168.55.110:5000/";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<bool> PingServer()
    {
        var response = await GetClient().GetAsync("/ping");

        response.EnsureSuccessStatusCode();

        return true;
    }

    public async Task<bool> UploadData(UploadData request)
    {
        try
        {
            var response = await GetClient().PostAsync("Client/UploadData",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<bool>();
        }
        catch
        {
            return false;
        }
    }

    public async Task<NewData?> GetNewData(DateTime? lastExchange)
    {
        try
        {
            var response = await GetClient().GetAsync($"Client/GetNewData?lastExchange={lastExchange}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<NewData>(json);
        }
        catch
        {
            return null;
        }
    }

    private static HttpClient GetClient()
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
        var response = await GetClient().PostAsJsonAsync("/Auth/Login", authModel, JsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<string>(JsonSerializerOptions);

        return token;
    }


    public async Task<List<Theme>?> GetThemesAsync()
    {
        var response = await GetClient().GetAsync("Client/GetThemes");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<Theme>>(json, JsonSerializerOptions);
    }


    public async Task<List<TaskForTest>?> GetTasksForThemeAsync(int themeId, int userId)
    {
        var response = await GetClient().PostAsJsonAsync($"/Client/GetTasksForTheme", new { themeId, userId },
            JsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<TaskForTest>>(JsonSerializerOptions);
    }

    public async Task<List<Lesson>?> GetLessonsForThemeAsync(int themeId)
    {
        var response = await GetClient().GetAsync($"/Client/GetLessonsForTheme?themeId={themeId}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Lesson>>(JsonSerializerOptions);
    }

    public async Task<bool> RegisterUser(RegisterModel request)
    {
        var response = await GetClient().PostAsJsonAsync("/Auth/Register", request, JsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<bool>(JsonSerializerOptions);
    }

    public async Task<Test?> GenerateTest(TestTypes testType, int userId, int? themeId = null)
    {
        var response = await GetClient()
            .GetAsync($"/Client/GenerateTest?testType={testType}&themeId={themeId}&userId={userId}");
        response.EnsureSuccessStatusCode();

        var temp = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Test>(temp);

        return result;
    }

    public async Task<ProfileInfo?> GetProfileInfo(int userId)
    {
        var response = await GetClient().GetAsync($"/Client/GetProfileInfo?userId={userId}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProfileInfo>(JsonSerializerOptions);
    }

    public async Task<bool> SaveAnswer(int userId, int taskId, bool isCorrect)
    {
        var response =
            await GetClient()
                .PostAsync($"/Client/SaveAnswer?userId={userId}&taskId={taskId}&isCorrect={isCorrect}", null);
        response.EnsureSuccessStatusCode();

        return true;
    }

    public async Task<bool> SaveAnswers(int userId, List<UserAnswer> answers)
    {
        var payload = new
        {
            userId,
            answers
        };

        var response = await GetClient()
            .PostAsJsonAsync("/Client/SaveAnswers", payload, JsonSerializerOptions);

        response.EnsureSuccessStatusCode();
        return true;
    }

    public static string? GetAbsoluteFilePath(string? filePath)
    {
        return filePath is null ? null : $"{ServerAddress}{filePath}";
    }
}