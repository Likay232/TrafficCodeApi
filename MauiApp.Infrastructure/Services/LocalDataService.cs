using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Models.Requests;
using MauiApp.Infrastructure.Models.Responses;
using MauiApp.Infrastructure.Models.Storage;
using MauiApp.Infrastructure.Models.Сomponents;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MauiApp.Infrastructure.Services;

public class LocalDataService(DataComponent component)
{
    public static async System.Threading.Tasks.Task SetUserInfo(string authToken, string refreshToken)
    {
        await SecureStorage.Default.SetAsync("auth_token", authToken);
        await SecureStorage.Default.SetAsync("refresh_token", refreshToken);
            
        if (string.IsNullOrEmpty(authToken)) return;
        
        var claims = TokenService.DecodeClaims(authToken);
            
        if (claims.TryGetValue("nameid", out var userId))
        { 
            Preferences.Default.Set("user_id", Convert.ToInt32(userId));
        }

        if (claims.TryGetValue("unique_name", out var username))
        {
            Preferences.Default.Set("username", username);
        }
    }
    
    public DateTime? GetLastExchange()
    {
        return component.ExchangeHistory
            .OrderBy(h => h.ExchangedAt)
            .LastOrDefault()?.ExchangedAt;
    }

    public async Task<bool> SaveExchange()
    {
        return await component.Insert(new ExchangeHistory()
        {
            ExchangedAt = DateTime.UtcNow,
        });
    }
    
    public UploadData GetDataToUpload()
    {
        var userId = Preferences.Get("user_id", 0);
        var lastExchange = GetLastExchange();
        var uploadData = new UploadData();

        if (lastExchange is null) 
            return uploadData;

        var completedTasks = component.CompletedTasks
            .Where(u => u.ModifiedAt > lastExchange)
            .ToList();

        var progresses = component.Progresses
            .Where(u => u.ModifiedAt > lastExchange)
            .ToList();

        Parallel.Invoke(
            () =>
            {
                completedTasks.Where(c => c.UserId == 0)
                    .ToList()
                    .ForEach(c => c.UserId = userId);
            },
            () =>
            {
                progresses.Where(p => p.UserId == 0)
                    .ToList()
                    .ForEach(p => p.UserId = userId);
            }
        );

        uploadData.CompletedTasks = completedTasks;
        uploadData.Progresses = progresses;

        return uploadData;
    }

    public static string? GetLocalPath(string? path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        
        var fileName = Path.GetFileName(path);
        return Path.Combine(FileSystem.AppDataDirectory, fileName);
    }

    public async Task<bool> ProcessNewData(NewData newData)
    {
        await component.ClearTableAsync<Models.Storage.Theme>();
        await component.ClearTableAsync<Models.Storage.Task>();
        await component.ClearTableAsync<Models.Storage.Lesson>();
        await component.ClearTableAsync<Progress>();
        await component.ClearTableAsync<CompletedTask>();

        await component.BulkInsertAsync(newData.Themes);

        foreach (var task in newData.Tasks)
        {
            if (string.IsNullOrEmpty(task.FilePath)) continue;
            
            var bytes = await ApiService.GetFileBytes(task.FilePath);
            
            if (bytes is null) continue;
            
            var localPath = GetLocalPath(task.FilePath);

            if (localPath is null) continue;
            
            await File.WriteAllBytesAsync(localPath, bytes);
            
            task.FilePath = localPath;
        }
        
        await component.BulkInsertAsync(newData.Tasks);
        await component.BulkInsertAsync(newData.Lessons);
        await component.BulkInsertAsync(newData.Progresses);
        
        return true;
    }
    
    public async Task<string?> Login()
    {
        return await SecureStorage.GetAsync("refresh_token");
    }
    
    public async Task<List<Models.DTO.Theme>?> GetThemesAsync()
    {
        return await component.Themes.Select(t => new Models.DTO.Theme
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
        }).ToListAsync();
    }

    public async Task<List<TaskForTest>> GetTasksForTheme(int userId, int themeId)
    {
        if (!await component.Themes.AnyAsync(t => t.Id == themeId))
            throw new Exception("Тема не найдена.");

        var completed = await component.CompletedTasks
            .Where(ct => ct.UserId == userId)
            .ToListAsync();

        var tasks = await component.Tasks
            .Include(t => t.Theme)
            .Where(t => t.ThemeId == themeId)
            .ToListAsync();

        return tasks.Select(t =>
        {
            var completedTask = completed.FirstOrDefault(ct => ct.TaskId == t.Id);

            return new TaskForTest()
            {
                Id = t.Id,
                ThemeId = t.ThemeId,
                Hint = t.Hint,
                Text = t.Text,
                CorrectAnswer = t.CorrectAnswer,
                DifficultyLevel = t.DifficultyLevel,
                FilePath = t.FilePath,
                IsCorrect = completedTask?.IsCorrect ?? null,
                AnswerVariants = JsonConvert.DeserializeObject<List<string?>>(t.AnswerVariants) ?? new List<string?>()
            };
        }).ToList();
    }

    public async Task<List<Models.DTO.Lesson>> GetLessonsForTheme(int themeId)
    {
        return await component.Lessons
            .Include(l => l.Theme)
            .Where(l => l.ThemeId == themeId)
            .Select(l => new Models.DTO.Lesson
            {
                Text = l.Text,
                Link = l.Link,
                ThemeId = l.ThemeId,
            })
            .ToListAsync();
    }
    
    public async Task<bool> SaveAnswers(int userId, List<UserAnswer> answers)
    {
        var tasks = new List<CompletedTask>();
        
        foreach (var answer in answers)
        {
            var task = component.Tasks.FirstOrDefault(t => t.Id == answer.TaskId);

            if (task is null)
                throw new Exception($"Не найдено задание с id {answer.TaskId}");

            var completedTask = new CompletedTask()
            {
                UserId = userId,
                TaskId = answer.TaskId,
                IsCorrect = answer.IsCorrect,
                CompletedAt = DateTime.UtcNow,
            };

            await ChangeUserProgress(task, userId, answer.IsCorrect);
            
            tasks.Add(completedTask);
        }

        await component.BulkInsertAsync(tasks);
        
        return true;
    }

    public async Task<bool> SaveAnswer(int userId, int taskId, bool isCorrect)
    {
        var task = component.Tasks.FirstOrDefault(t => t.Id == taskId);

        if (task is null)
            throw new Exception($"Не найдено задание с id {taskId}");

        var completedTask = new CompletedTask()
        {
            UserId = userId,
            TaskId = taskId,
            IsCorrect = isCorrect,
            CompletedAt = DateTime.UtcNow,
        };

        await ChangeUserProgress(task, userId, isCorrect);
        
        return await component.Insert(completedTask);
    }
    
    private double GetExperience(bool isCorrect, int difficultyLevel, int currentLevel)
    {
        return !isCorrect ? 10 * (double)difficultyLevel / currentLevel : 10 * (double)currentLevel / difficultyLevel;
    }

    private async System.Threading.Tasks.Task ChangeUserProgress(Models.Storage.Task task, int userId, bool isCorrect)
    {
        var currentProgress =
            component.Progresses.FirstOrDefault(p => p.UserId == userId && p.ThemeId == task.ThemeId);

        bool toUpdate = true;

        if (currentProgress?.Level == 5) return;

        if (currentProgress is null)
        {
            toUpdate = false;

            currentProgress = new Progress
            {
                UserId = userId,
                ThemeId = task.ThemeId,
                Level = 1,
                AmountToLevelUp = 100
            };
        }

        var experience = GetExperience(isCorrect, task.DifficultyLevel, currentProgress.Level);

        currentProgress.AmountToLevelUp -= experience;

        if (currentProgress.AmountToLevelUp <= 0)
        {
            currentProgress.Level += 1;
            currentProgress.AmountToLevelUp = 100 * Math.Pow(2, currentProgress.Level);
        }
        
        currentProgress.ModifiedAt = DateTime.UtcNow;

        if (toUpdate) await component.Update(currentProgress);
        else await component.Insert(currentProgress);
    }

    public async Task<int> GetTaskAmount()
    {
        return await component.Tasks.CountAsync();
    }
    
        public async Task<Test> GenerateTest(TestTypes testType, int userId, int? themeId = null)
    {
        switch (testType)
        {
            case TestTypes.Themes:
                return await GenerateTestForTheme(themeId);
            case TestTypes.Marathon:
                return await GenerateTestForMarathon();
            case TestTypes.Exam:
                return await GenerateTestForExam();
            case TestTypes.ChallengingQuestions:
                return await GenerateTestForChallengingQuestions(userId);
            default: throw new ArgumentOutOfRangeException(nameof(testType));
        }
    }

    private async Task<Test> GenerateTestForTheme(int? themeId)
    {
        var tasks = await component.Tasks
            .Where(t => t.ThemeId == themeId)
            .Select(t => new TaskForTest
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

        return new Test()
        {
            Tasks = tasks
        };
    }

    private async Task<Test> GenerateTestForMarathon()
    {
        var tasks = await component.Tasks
            .OrderBy(t => EF.Functions.Random())
            .Take(800)
            .Select(t => new TaskForTest()
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

        return new Test()
        {
            Tasks = tasks
        };
    }

    private async Task<Test> GenerateTestForExam()
    {
        var themeIds = await component.Themes
            .OrderBy(t => EF.Functions.Random())
            .Select(t => t.Id)
            .Take(4)
            .ToListAsync();

        var test = new Test();

        foreach (var themeId in themeIds)
        {
            var tasks = await component.Tasks
                .Where(t => t.ThemeId == themeId)
                .OrderBy(t => EF.Functions.Random())
                .Take(10)
                .Select(t => new TaskForTest
                {
                    Id = t.Id,
                    ThemeId = t.ThemeId,
                    Text = t.Text,
                    CorrectAnswer = t.CorrectAnswer,
                    DifficultyLevel = t.DifficultyLevel,
                    FilePath = t.FilePath,
                    AnswerVariants =
                        JsonConvert.DeserializeObject<List<string?>>(t.AnswerVariants)
                        ?? new List<string?>(),
                    Hint = t.Hint
                })
                .ToListAsync();

            var tasksForTest = tasks.Take(5).ToList();
            var additionalQuestionsForTheme = tasks.TakeLast(5).ToList();

            test.Tasks.AddRange(tasksForTest);
            test.AdditionalQuestions[themeId] = additionalQuestionsForTheme;
        }

        return test;
    }

    private async Task<Test> GenerateTestForChallengingQuestions(int userId)
    {
        var userLevels = component.Progresses
            .Where(p => p.UserId == userId)
            .ToDictionary(p => p.ThemeId, p => p.Level);
        
        var grouped = await component.CompletedTasks
            .Where(ct => ct.UserId == userId && ct.IsCorrect == false)
            .GroupBy(ct => new
            {
                ct.TaskId,
                ct.Task.ThemeId,
                ct.Task.DifficultyLevel
            })
            .Select(g => new
            {
                g.Key.TaskId,
                g.Key.ThemeId,
                Difficulty = g.Key.DifficultyLevel,
                WrongCount = g.Count()
            })
            .ToListAsync();

        var mostChallengingQuestions = grouped
            .Select(g =>
            {
                var userLevel = userLevels.TryGetValue(g.ThemeId, out var lvl)
                    ? lvl
                    : 1;

                var distance = Math.Abs(g.Difficulty - userLevel);
                var isAbove = g.Difficulty > userLevel ? 1 : 0;

                return new
                {
                    g.TaskId,
                    g.WrongCount,
                    Distance = distance,
                    IsAbove = isAbove
                };
            })
            .OrderBy(g => g.Distance)
            .ThenBy(g => g.IsAbove)
            .ThenByDescending(g => g.WrongCount)
            .Take(20)
            .Select(g => g.TaskId)
            .ToList();

        var tasks = new List<TaskForTest>();
        foreach (var taskId in mostChallengingQuestions)
        {
            var task = component.Tasks.FirstOrDefault(t => t.Id == taskId);
            
            if (task == null) continue;
            
            tasks.Add(new TaskForTest
            {
                Id = task.Id,
                ThemeId = task.ThemeId,
                Text = task.Text,
                CorrectAnswer = task.CorrectAnswer,
                DifficultyLevel = task.DifficultyLevel,
                FilePath = task.FilePath,
                AnswerVariants = JsonConvert.DeserializeObject<List<string?>>(task.AnswerVariants) ?? new List<string?>(),
                Hint = task.Hint
            });
        }
        
        return new Test
        {
            Tasks = tasks
        };
    }
}