using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Requests;
using MauiApp.Infrastructure.Models.Responses;
using Newtonsoft.Json;
using Plugin.Firebase.CloudMessaging;

namespace MauiApp.Infrastructure.Services;

public class ApiService
{
    private const string ServerAddress = "http://192.168.55.108:5000/";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task<byte[]?> GetFileBytes(string? path)
    {
        try
        {
            if (path is null) return null;
            
            var bytes = await GetClient().GetByteArrayAsync(path);
            
            return bytes;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static async Task<bool> RegisterDevice(string? deviceToken = null)
    {
        try
        {
            var userId = Preferences.Get("user_id", 0);
            deviceToken ??= await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            var payload = new
            {
                UserId = userId,
                DeviceToken = deviceToken
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await GetClient().PostAsync("/Client/RegisterDevice", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return !string.IsNullOrEmpty(responseContent) &&
                   JsonConvert.DeserializeObject<bool>(responseContent);

        }
        catch (Exception)
        {
            return false;
        }
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
    
    public static async Task<TData?> GetData<TData>(string path)
    {
        var authToken = await SecureStorage.GetAsync("auth_token");
        if (TokenService.ShouldRefresh(authToken) && !await TokenService.RefreshToken())
        {
            return default;
        }

        var response = await GetClient().GetAsync(path);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        
        return string.IsNullOrEmpty(json) ? default : JsonConvert.DeserializeObject<TData>(json);
    }

    public static async Task<TData?> PostData<TData, TBody>(string path, TBody? body = default)
    {
        var authToken = await SecureStorage.GetAsync("auth_token");
        if (TokenService.ShouldRefresh(authToken) && !await TokenService.RefreshToken())
        {
            return default;
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
        
        return string.IsNullOrEmpty(json) ? default : JsonConvert.DeserializeObject<TData>(json);
    }

    public static async Task<bool> PingServer()
    {
        try
        {
            var response = await GetClient().GetAsync("/ping");
            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception)
        {
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
    
    public async Task<bool> RegisterUser(RegisterModel request)
    {
        var response = await GetClient().PostAsJsonAsync("/Auth/Register", request, JsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<bool>(JsonSerializerOptions);
    }

    public static string? GetAbsoluteFilePath(string? filePath)
    {
        return filePath is null ? null : $"{ServerAddress}{filePath}";
    }
}