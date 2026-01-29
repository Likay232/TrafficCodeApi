using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Storage;
using MauiApp.Infrastructure.Models.Сomponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MauiApp.Infrastructure.Services;

public class LocalDbService(DataComponent component)
{
    public async Task<string?> Login(AuthModel login)
    {
        var user = component.Users.FirstOrDefault(u =>
            u.Username == login.Username && u.Password == login.Password);

        if (user == null || user.IsBlocked)
            return null;

        user.LastLogin = DateTime.Now;
        await component.Update(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "super_secret_key_12345";

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, login.Username ?? "")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }
    
    public async Task<bool> Register(RegisterModel request)
    {
        var user = await component.Users.FirstOrDefaultAsync(u =>
            u.Username == request.Username);

        if (user != null)
            throw new Exception("Имя пользователя занято.");

        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Password = request.Password,
            LastLogin = DateTime.MaxValue,
            IsSynced = false
        };

        return await component.Insert(newUser);
    }
}