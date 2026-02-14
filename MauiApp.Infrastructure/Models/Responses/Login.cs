namespace MauiApp.Infrastructure.Models.Responses;

public class Login
{
    public string AccessToken { get; set; } =  string.Empty;
    public string RefreshToken { get; set; } =  string.Empty;
}