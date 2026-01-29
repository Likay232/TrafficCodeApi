namespace MauiApp.Services;

using System.IdentityModel.Tokens.Jwt;

public static class TokenParseService
{
    public static IDictionary<string, string> DecodeClaims(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);

        return token.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.First().Value);
    }

    public static bool IsExpired()
    {
        var expiresString = Preferences.Default.Get("exp", "0");
        var expires = Convert.ToInt64(expiresString);

        if (expires == 0) return true;

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expires);

        return expirationTime <= DateTimeOffset.UtcNow;
    }
}