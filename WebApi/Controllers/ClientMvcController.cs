using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.Models.Enums;
using WebApi.Infrastructure.Models.Requests;
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

    public async Task<IActionResult> Tasks(int themeId)
    {
        var userIdStr = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdStr == null)
            throw new Exception("No user claim found");
        
        int.TryParse(userIdStr, out var userId);

        var tasks = 
            await service.GetTasksForTheme(new GetTasks() {ThemeId = themeId, UserId = userId});

        return View(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> Test(int themeId, TestTypes testType, Guid testId)
    {
        var userIdStr = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdStr == null)
            throw new Exception("No user claim found");
        
        int.TryParse(userIdStr, out var userId);

        var test = await service.GenerateTest(testType, themeId, userId);
        
        ViewBag.TestType = testType;
        ViewBag.TestId = testId;
        ViewBag.ThemeId = themeId;
        
        return View(test);
    }

    [HttpGet]
    public IActionResult StartTest(int themeId, TestTypes testType)
    {
        var testId = Guid.NewGuid();
        
        return RedirectToAction(nameof(Test), new { themeId, testType, testId });
    }

    [HttpPost]
    public async Task<IActionResult> SaveAnswer([FromBody] SaveAnswer req)
    {
        try
        {
            var userIdStr = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdStr == null)
                throw new Exception("No user claim found");
        
            int.TryParse(userIdStr, out var userId);

            await service.SaveAnswer(userId, req.TaskId, req.IsCorrect);

            return StatusCode(200);

        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}