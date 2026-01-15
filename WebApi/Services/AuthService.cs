using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Infrastructure.Components;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Infrastructure.Models.Storage;

namespace WebApi.Services;

public class AuthService(DataComponent component)
{
    public async Task<string?> Login(Login request)
    {
        string? role;
        int userId;

        if (request is { UserName: "admin", Password: "admin123" })
        {
            role = "Admin";
            userId = 0;
        }
        else
        {
            var user = component.Users.FirstOrDefault(u =>
                u.Username == request.UserName && u.Password == request.Password);

            if (user == null || user.IsBlocked)
                return null;

            role = "Client";
            userId = user.Id;
            user.LastLogin = DateTime.Now;
            await component.Update(user);
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "super_secret_key_12345";

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId.ToString()),
            new (ClaimTypes.Name, request.UserName),
            new (ClaimTypes.Role, role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(role == "Admin" ? 1 : 3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public async Task<bool> Register(Register request)
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
            LastLogin = DateTime.MaxValue
        };

        return await component.Insert(newUser);
    }
}