using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Infrastructure.Components;
using WebApi.Infrastructure.Models.DTO;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Infrastructure.Models.Storage;
using Task = WebApi.Infrastructure.Models.Storage.Task;

namespace WebApi.Services;

public class AdminService(DataComponent component, IWebHostEnvironment env)
{
    public async Task<List<UserDto>> GetUsers()
    {
        return await component.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsBlocked = u.IsBlocked,
                Username = u.Username,
            })
            .ToListAsync();
    }

    public async Task<UserDto?> GetUser(int userId)
    {
        var user = await component.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new Exception("Пользователь не найден");
        
        var themesStatistic = await GetThemeStatisticForUser(userId);

        return new UserDto()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            IsBlocked = user.IsBlocked,
            LastName = user.LastName,
            Password = user.Password,
            ThemesStatistics = themesStatistic,
            Username = user.Username,
            LastLogin = user.LastLogin
        };
    }

    public async Task<bool> SwitchBlockState(int userId)
    {
        var userToBlock = await component.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (userToBlock == null)
            throw new Exception("Пользователь не найден.");

        userToBlock.IsBlocked = !userToBlock.IsBlocked;

        return await component.Update(userToBlock);
    }

    public async Task<bool> ChangeUserPassword(ChangePasswordAdmin request)
    {
        var userEntry = await component.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (userEntry == null)
            throw new Exception("Пользователь с заданным Id не найден");

        userEntry.Password = request.NewPassword;

        return await component.Update(userEntry);
    }

    public async Task<bool> DeleteUser(int userId)
    {
        return await component.DeleteUser(userId);
    }

    public async Task<List<ThemeDto>> GetThemes()
    {
        return await component.Themes.Select(t => new ThemeDto
        {
            Id = t.Id,
            Description = t.Description,
            Title = t.Title
        }).ToListAsync();
    }

    public async Task<List<TaskDto>> GetTasksForTheme(int themeId)
    {
        return await component.Tasks
            .Where(t => t.ThemeId == themeId)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                ThemeId = t.ThemeId,
                Text = t.Text,
                CorrectAnswer = t.CorrectAnswer,
                DifficultyLevel = t.DifficultyLevel,
                FilePath = t.FilePath,
                AnswerVariants = JsonConvert.DeserializeObject<List<string?>>(t.AnswerVariants) ?? new List<string?>(),
                Hint = t.Hint
            })
            .ToListAsync();
    }

    public async Task<TaskDto?> GetTaskById(int taskId)
    {
        var task = await component.Tasks.Include(t => t.Theme)
            .FirstOrDefaultAsync(t => t.Id == taskId);
        return task == null
            ? null
            : new TaskDto
            {
                Id = task.Id,
                Text = task.Text,
                CorrectAnswer = task.CorrectAnswer,
                DifficultyLevel = task.DifficultyLevel,
                ThemeId = task.ThemeId,
                ThemeName = task.Theme.Title,
                FilePath = task.FilePath,
                AnswerVariants = JsonConvert.DeserializeObject<List<string?>>(task.AnswerVariants) ?? new List<string?>(),
                Hint = task.Hint
            };
    }

    public async Task<bool> CreateNewTheme(CreateTheme request)
    {
        var newTheme = new Theme
        {
            Title = request.Title,
            Description = request.Description,
        };

        return await component.Insert(newTheme);
    }

    public async Task<bool> AddTaskForTheme(TaskDto taskToAdd)
    {
        if (!component.Themes.Any(t => t.Id == taskToAdd.ThemeId))
            throw new Exception("Тема с таким Id не найдена.");

        var newTask = new Task
        {
            ThemeId = taskToAdd.ThemeId,
            Text = taskToAdd.Text,
            CorrectAnswer = taskToAdd.CorrectAnswer,
            DifficultyLevel = taskToAdd.DifficultyLevel,
            FilePath = taskToAdd.FilePath,
            AnswerVariants = JsonConvert.SerializeObject(taskToAdd.AnswerVariants),
            Hint = taskToAdd.Hint
        };

        return await component.Insert(newTask);
    }

    public async Task<bool> EditTaskForTheme(TaskDto updatedTask)
    {
        if (!component.Themes.Any(t => t.Id == updatedTask.ThemeId))
            throw new Exception("Тема с таким Id не найдена.");

        var taskToEdit = component.Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);

        if (taskToEdit == null)
            throw new Exception("Задание с таким Id не найдено.");

        taskToEdit.Text = updatedTask.Text;
        taskToEdit.CorrectAnswer = updatedTask.CorrectAnswer;
        taskToEdit.DifficultyLevel = updatedTask.DifficultyLevel;
        taskToEdit.FilePath = updatedTask.FilePath;
        taskToEdit.ThemeId = updatedTask.ThemeId;
        taskToEdit.Hint = updatedTask.Hint;
        taskToEdit.AnswerVariants = JsonConvert.SerializeObject(updatedTask.AnswerVariants);

        return await component.Update(taskToEdit);
    }

    public async Task<bool> DeleteTaskForTheme(int taskId)
    {
        return await component.Delete<Task>(taskId);
    }

    public async Task<bool> AddLessonForTheme(LessonDto lessonToAdd)
    {
        if (!component.Themes.Any(t => t.Id == lessonToAdd.ThemeId))
            throw new Exception("Тема с таким Id не найдена.");

        var newLesson = new Lesson
        {
            ThemeId = lessonToAdd.ThemeId,
            Text = lessonToAdd.Text,
            Link = lessonToAdd.Link
        };

        return await component.Insert(newLesson);
    }

    public async Task<List<TaskDto>> GetTasks()
    {
        return await component.Tasks
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Text = t.Text,
                CorrectAnswer = t.CorrectAnswer,
                DifficultyLevel = t.DifficultyLevel,
                FilePath = t.FilePath,
                ThemeName = GetThemeName(t.ThemeId),
                ThemeId = t.ThemeId,
                AnswerVariants = JsonConvert.DeserializeObject<List<string?>>(t.AnswerVariants) ?? new List<string?>()
            })
            .ToListAsync();
    }

    public async Task<string> CreateTest(CreateTest request)
    {
        var newTest = new Test
        {
            Title = request.Title,
        };

        if (!await component.Insert(newTest))
            throw new Exception("Ошибка создания теста.");

        List<int> failedTaskIds = new List<int>();

        foreach (var taskId in request.TaskIds)
        {
            var result = await component.Insert(new TestTask
            {
                TestId = newTest.Id,
                TaskId = taskId
            });

            if (!result)
            {
                failedTaskIds.Add(taskId);
            }
        }

        if (failedTaskIds.Count > 0)
        {
            return $"Не удалось добавить задачи с ID: {string.Join(", ", failedTaskIds)}";
        }

        return $"Тест успешно создан. ID: {newTest.Id}";
    }

    public async Task<string?> SaveFileToRepo(IFormFile file, string fileName)
    {
        var path = Path.Combine(env.ContentRootPath, "wwwroot/files", fileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/files/{fileName}";
    }

    public async Task<byte[]?> GetFileBytes(string fileName)
    {
        var filePath = Path.Combine(env.ContentRootPath, "wwwroot/files", fileName);

        if (!File.Exists(filePath))
            return null;

        var fileBytes = await File.ReadAllBytesAsync(filePath);

        return fileBytes;
    }

    public async Task<List<ThemesStatistic>> GetThemeStatisticForUser(int userId)
    {
        var progresses = await component.Progresses.ToListAsync();
        
        var tasks = await component.Tasks
            .Include(t => t.Theme)
            .ToListAsync();
        var completedTasks = await component.CompletedTasks
            .Where(x => x.UserId == userId)
            .ToDictionaryAsync(x => x.TaskId, x => x.IsCorrect == true);

        var statistic = tasks
            .GroupBy(t => new { t.ThemeId, t.Theme.Title })
            .ToDictionary(g => g.Key, g =>
            {
                var total = g.Count();
                var solved = g.Count(t => completedTasks.ContainsKey(t.Id));
                var solvedCorrect = g.Count(t => completedTasks.TryGetValue(t.Id, out var cor) && cor);

                var solvedPercent = total == 0 ? 0.0 : (double)solved / total * 100;
                var correctPercent = solved == 0 ? 0.0 : (double)solvedCorrect / solved * 100;

                return new ThemesStatistic
                {
                    SolvedPercent = solvedPercent,
                    SolvedCorrectPercent = correctPercent,
                    ThemeId = g.Key.ThemeId,
                    ThemeName = g.Key.Title,
                    Level = progresses.FirstOrDefault(p => p.ThemeId == g.Key.ThemeId)?.Level ?? 1
                };
            });

        return statistic.Values.ToList();
    }
    
    public string GetThemeName(int themeId)
    {
        return component.Themes.First(t => t.Id == themeId).Title;
    }

    public async Task<bool> DeleteTheme(int themeId)
    {
        return await component.Delete<Theme>(themeId);
    }

    public async Task<bool> EditTheme(ThemeDto updatedTheme)
    {
        var themeToUpdate = await component.Themes.FirstOrDefaultAsync(t => t.Id == updatedTheme.Id);

        if (themeToUpdate == null)
            throw new Exception("Тема с таким Id не найдена");

        themeToUpdate.Title = updatedTheme.Title;
        themeToUpdate.Description = updatedTheme.Description;

        return await component.Update(themeToUpdate);
    }

    public async Task<ThemeDto> GetThemeById(int themeId)
    {
        var theme = await component.Themes
            .FirstOrDefaultAsync(t => t.Id == themeId);

        if (theme == null)
            throw new Exception("Тема с таким id не найдена");

        return new ThemeDto()
        {
            Id = themeId,
            Title = theme.Title,
            Description = theme.Description,
        };
    }

    public async Task<List<LessonDto>> GetLessons(int themeId)
    {
        return await component.Lessons
            .Include(l => l.Theme)
            .Where(l => l.ThemeId == themeId)
            .Select(l => new LessonDto()
            {
                Id = l.Id,
                ThemeId = l.ThemeId,
                ThemeName = l.Theme.Title,
                Text = l.Text,
                Link = l.Link,
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteLesson(int lessonId)
    {
        var lessonToDelete = await component.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lessonToDelete is null)
            throw new Exception("Теоретический материал с таким id не найден");

        return await component.Delete<Lesson>(lessonId);
    }

    public async Task<LessonDto> GetLessonById(int lessonId)
    {
        var lesson = await component.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null) throw new Exception("Не найден теоретический материал с данным id.");

        return new LessonDto()
        {
            Id = lessonId,
            ThemeId = lesson.ThemeId,
            Text = lesson.Text,
            Link = lesson.Link,
        };
    }

    public async Task<bool> UpdateLesson(LessonDto updatedLesson)
    {
        var lessonToUpdate = await component.Lessons.FirstOrDefaultAsync(l => l.Id == updatedLesson.Id);

        if (lessonToUpdate == null) throw new Exception("Не найден теоретический материал с данным id.");
        
        lessonToUpdate.Text = updatedLesson.Text;
        lessonToUpdate.Link = updatedLesson.Link;
        
        return await component.Update(lessonToUpdate);
    }
}