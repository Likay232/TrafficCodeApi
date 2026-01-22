using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.Models.DTO;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Services;

namespace WebApi.Controllers;

// [Authorize(Roles = "Admin")]
[Route("[controller]/[action]")]
public class AdminController(AdminService service) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        return View(await service.GetUsers());
    }

    [HttpGet]
    public async Task<IActionResult> SwitchBlockState(int userId)
    {
        await service.SwitchBlockState(userId);
        
        return RedirectToAction(nameof(Users));
    }


    [HttpPost]
    public async Task<IActionResult> ChangePasswordForUser([FromBody] ChangePasswordAdmin request)
    {
        try
        {
            var result = await service.ChangeUserPassword(request);
            
            return StatusCode(200, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        await service.DeleteUser(userId);
        
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public async Task<IActionResult> Themes()
    {
        return View(await service.GetThemes());
    }
    
    [HttpGet]
    public IActionResult AddTheme()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddTheme([FromForm] CreateTheme request)
    {
        try
        {
            if (request.Description == null) request.Description = string.Empty; 
            
            await service.CreateNewTheme(request);
            return RedirectToAction(nameof(Themes));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка при создании темы: {e.Message}");
            return View();
        }
    }

    [HttpGet]
    public IActionResult AddLesson(int themeId)
    {
        var model = new LessonDto {ThemeId = themeId, ThemeName = service.GetThemeName(themeId)};
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddLesson([FromForm] LessonDto lessonToAdd)
    {
        try
        {
            await service.AddLessonForTheme(lessonToAdd);
            return RedirectToAction(nameof(Lessons), new {themeId = lessonToAdd.ThemeId});
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка при добавлении материала: {e.Message}");
            return View(lessonToAdd);
        }
    }
    
    [HttpGet]
    public IActionResult AddTask(int themeId)
    {
        var model = new TaskDto
        {
            ThemeId = themeId,
            ThemeName = service.GetThemeName(themeId),
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddTask([FromForm]TaskDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (model.Image is { Length: > 0 })
            {
                var ext = Path.GetExtension(model.Image.FileName);
                var fileName = $"{Guid.NewGuid():N}{ext}";

                var path = await service.SaveFileToRepo(model.Image, fileName); 
                
                if (path != null)
                    model.FilePath = path;
                else
                {
                    ModelState.AddModelError("", "Не удалось добавить задание");
                    return View(model);
                }
            }

            var success = await service.AddTaskForTheme(model);

            if (success)
                return RedirectToAction(nameof(Tasks), new { themeId = model.ThemeId });

            ModelState.AddModelError("", "Не удалось добавить задание");
            return View(model);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка: {e.Message}");
            return View(model);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Tasks(int themeId)
    {
        return View(await service.GetTasksForTheme(themeId));
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteTask(int taskId, int themeId)
    {
        try
        {
            var result = await service.DeleteTaskForTheme(taskId);
            if (result)
            {
                return RedirectToAction(nameof(Tasks), new { themeId });
            }

            TempData["Error"] = "Не удалось удалить задание.";
            return RedirectToAction(nameof(Tasks), new { themeId });
        }
        catch (Exception e)
        {
            TempData["Error"] = $"Ошибка при удалении задания: {e.Message}";
            return RedirectToAction(nameof(Tasks), new { themeId });
        }
    }

    
    [HttpGet]
    public async Task<IActionResult> EditTask(int taskId)
    {
        var task = await service.GetTaskById(taskId);
        if (task == null)
            return NotFound();

        return View(task);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask([FromForm] TaskDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (model.Image is { Length: > 0 })
            {
                var ext = Path.GetExtension(model.Image.FileName);
                var fileName = $"{Guid.NewGuid():N}{ext}";

                var path = await service.SaveFileToRepo(model.Image, fileName); 
                
                if (path != null)
                    model.FilePath = path;
                else
                {
                    ModelState.AddModelError("", "Не удалось обновить задание.");
                    return View(model);
                }
            }
            
            var result = await service.EditTaskForTheme(model);
            if (result)
                return RedirectToAction(nameof(Tasks), new { themeId = model.ThemeId });

            ModelState.AddModelError("", "Не удалось обновить задание.");
            return View(model);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка: {e.Message}");
            return View(model);
        }
    }

    // [HttpGet]
    // public async Task<IActionResult> CreateTest()
    // {
    //     var tasks = await service.GetTasks();
    //     ViewBag.Tasks = tasks ?? new List<TaskDto>();
    //     return View(new CreateTest());
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> CreateTest([FromForm] CreateTest request, string taskIdsStr)
    // {
    //     try
    //     {
    //         if (string.IsNullOrWhiteSpace(taskIdsStr))
    //         {
    //             ModelState.AddModelError("", "Не выбрано ни одного задания.");
    //             var tasks = await service.GetTasks();
    //             ViewBag.Tasks = tasks;
    //             return View(request);
    //         }
    //
    //         request.TaskIds = taskIdsStr
    //             .Split(',', StringSplitOptions.RemoveEmptyEntries)
    //             .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
    //             .Where(id => id.HasValue)
    //             .Select(id => id!.Value)
    //             .ToList();
    //
    //         await service.CreateTest(request);
    //         return RedirectToAction(nameof(Index));
    //     }
    //     catch (Exception e)
    //     {
    //         ModelState.AddModelError("", $"Ошибка при создании теста: {e.Message}");
    //         var tasks = await service.GetTasks();
    //         ViewBag.Tasks = tasks;  
    //         return View(request);
    //     }
    // }

    [HttpGet]
    public async Task<IActionResult> DownloadFileFromRepo(string filePath)
    {
        try
        {
            var fileBytes = await service.GetFileBytes(filePath);
            
            if (fileBytes == null) return NotFound();
            
            return File(fileBytes, "application/octet-stream", filePath);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ThemesStatistic>>> GetUserStatisticForTheme(int userId)
    {
        try
        {
            var statistic = await service.GetThemeStatisticForUser(userId);
            
            return StatusCode(200, statistic);
        }
        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteTheme(int themeId)
    {
        await service.DeleteTheme(themeId);
        
        return RedirectToAction(nameof(Themes));
    }

    [HttpGet]
    public async Task<IActionResult> EditTheme(int themeId)
    {
        var theme = await service.GetThemeById(themeId);
        
        return View(theme);
    }

    [HttpPost]
    public async Task<ActionResult> EditTheme([FromForm] ThemeDto model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var result = await service.EditTheme(model); 
        
        if (result)
            return RedirectToAction(nameof(Themes));

        ModelState.AddModelError("", "Не удалось обновить тему.");
        return View(model);
    }

    [HttpGet]
    public async Task<ActionResult> Lessons(int themeId)
    {
        ViewBag.ThemeId = themeId;
        ViewBag.ThemeName =  service.GetThemeName(themeId);
        var lessons = await service.GetLessons(themeId);

        return View(lessons);
    }

    [HttpGet]
    public async Task<ActionResult> DeleteLesson(int lessonId, int themeId)
    {
        await service.DeleteLesson(lessonId);
        return RedirectToAction(nameof(Lessons), new { themeId });
    }

    [HttpGet]
    public async Task<IActionResult> EditLesson(int lessonId)
    {
        var model = await service.GetLessonById(lessonId);
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditLesson(LessonDto updatedLesson)
    {
        await service.UpdateLesson(updatedLesson);
        
        return RedirectToAction(nameof(Lessons), new {themeId = updatedLesson.ThemeId});
    }

    public async Task<IActionResult> GetUser(int userId)
    {
        var model = await service.GetUser(userId);
        
        return View(model);
    }
}