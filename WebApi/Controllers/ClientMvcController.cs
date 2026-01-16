using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("[controller]/[action]")]
public class ClientMvcController(ClientService service) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Themes()
    {
        var themes = await service.GetThemes();
        
        return View(themes);
    }

    public async Task<IActionResult> Lessons(int themeId)
    {
        var lessons = await service.GetLessonsForTheme(themeId);
        
        return View(lessons);
    }

    public async Task<IActionResult> ProfileInfo()
    {
        var userIdStr = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdStr == null)
            throw new Exception("No user claim found");
        
        int.TryParse(userIdStr, out var userId);

        var profileInfo = await service.GetProfileInfo(userId);
        
        return View(profileInfo);
    }
}