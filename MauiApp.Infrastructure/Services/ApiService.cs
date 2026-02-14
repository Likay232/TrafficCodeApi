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
    private const string ServerAddress = "http://192.168.55.111:5000/";
    private static bool _available = true;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static bool IsAvailable() => _available;

    public static async Task<bool> PingServer()
    {
        try
        {
            var response = await GetClient().GetAsync("/ping");
            response.EnsureSuccessStatusCode();

            _available = true;
            return true;
        }
        catch (Exception)
        {
            _available = false;
            return false;
        }
    }

    public static async Task<(string AccessToken, string RefreshToken)?> RefreshToken(string? refreshToken = null)
    {
        refreshToken ??= await SecureStorage.GetAsync("refresh_token");

        var response = await GetClient().GetAsync($"/Client/Auth/Refresh?refreshToken={refreshToken}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonConvert.DeserializeObject<(string AccessToken, string RefreshToken)>(json);
    }

    public static async Task<TData?> GetData<TData>(string path)
    {
        var authToken = await SecureStorage.GetAsync("auth_token");
        if (TokenService.ShouldRefresh(authToken))
        {
            await TokenService.RefreshToken();
        }

        var response = await GetClient().GetAsync(path);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrEmpty(json))
            return default;

        return JsonConvert.DeserializeObject<TData>(json);
    }

    public static async Task<TData?> PostData<TData, TBody>(string path, TBody? body)
    {
        var authToken = await SecureStorage.GetAsync("auth_token");
        if (TokenService.ShouldRefresh(authToken))
        {
            await TokenService.RefreshToken();
        }

        HttpContent content;
        if (body is null)
        {
            content = new StringContent("", Encoding.UTF8, "application/json");
        }
        else
        {
            var serialized = JsonConvert.SerializeObject(body);
            content = new StringContent(serialized, Encoding.UTF8, "application/json");
        }

        var response = await GetClient().PostAsync(path, content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrEmpty(json))
            return default;

        return JsonConvert.DeserializeObject<TData>(json);
    }


    public async Task<bool> UploadData(UploadData request)
    {
        var authToken = await SecureStorage.GetAsync("auth_token");
        if (TokenService.ShouldRefresh(authToken))
        {
            await TokenService.RefreshToken();
        }

        var response = await GetClient().PostAsync("Client/UploadData",
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
       
        response.EnsureSuccessStatusCode();

        
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<NewData?> GetNewData(int userId)
    {
        var authToken = await SecureStorage.GetAsync("auth_token");
        if (TokenService.ShouldRefresh(authToken))
        {
            await TokenService.RefreshToken();
        }

        var response = await GetClient().GetAsync($"Client/GetNewData?userId={userId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<NewData>(json);
    }

    private static HttpClient GetClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var token = SecureStorage.GetAsync("auth_token").Result;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.BaseAddress = new Uri(ServerAddress);

        client.Timeout = TimeSpan.FromMinutes(1);
        return client;
    }

    public async Task<Login?> Login(AuthModel authModel)
    {
        var response = await GetClient().PostAsJsonAsync("/Auth/Login", authModel, JsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrEmpty(json))
            return null;

        var token = JsonConvert.DeserializeObject<Login>(json);

        return token;
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