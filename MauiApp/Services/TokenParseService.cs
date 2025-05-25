using System.Diagnostics;

namespace MauiApp.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
}