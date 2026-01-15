using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("[controller]/[action]")]
public class AuthMvcController(AuthService service) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View(new Login());
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new Register());
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromForm] Register request)
    {
        try
        {
            var registerResult = await service.Register(request);

            if (!registerResult)
                return View(request);

            return RedirectToAction("Login", "AuthMvc");
        }
        catch
        {
            return View(request);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromForm] Login request)
    {
        try
        {
            var token = await service.Login(request);

            if (string.IsNullOrEmpty(token))
            {
                return View(request);
            }
            
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,      
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(role == "Admin" ? 1 : 3)
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            return role switch
            {
                "Admin"  => RedirectToAction("Index", "Admin"),
                "Client" => RedirectToAction("Index", "ClientMvc"),
                _        => View(request)
            };
        }
        catch
        {
            return View(request);
        }
    }
}