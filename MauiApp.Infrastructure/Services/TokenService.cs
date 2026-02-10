using System.IdentityModel.Tokens.Jwt;

namespace MauiApp.Infrastructure.Services;

public static class TokenService
{
    public static IDictionary<string, string> DecodeClaims(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);

        return token.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.First().Value);
    }

    public static bool IsExpired(string? token)
    {
        if (token is null) return true;
        
        var expiresString = DecodeClaims(token)["exp"];
        var expires = Convert.ToInt32(expiresString);
        
        if (expires == 0) return true;

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expires);

        return expirationTime <= DateTimeOffset.UtcNow;
    }
    
    public static bool ShouldRefresh(string? token)
    {
        if (token is null)
            return true;

        var claims = DecodeClaims(token);

        if (!claims.TryGetValue("exp", out var expiresString))
            return true;

        if (!long.TryParse(expiresString, out var expires) || expires == 0)
            return true;

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expires);

        return expirationTime <= DateTimeOffset.UtcNow.AddMinutes(15);
    }

    public static async Task RefreshToken()
    {
        var tokens = await ApiService.RefreshToken();
        
        if (tokens is null && ApiService.IsAvailable())
        {
            await Shell.Current.GoToAsync("/AuthView");
            return;
        }
        
        if (tokens is not null)
            await LocalDataService.SetUserInfo(tokens.Value.AccessToken, tokens.Value.RefreshToken);
    }
}