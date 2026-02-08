using EFCore.BulkExtensions;
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
        var uploadData = new UploadData();
        
        var lastExchange = GetLastExchange();
        
        if (lastExchange is null) return uploadData;

        Parallel.Invoke(
            () => uploadData.Users = component.Users.Where(u => u.ModifiedAt > lastExchange).ToList(),
            () => uploadData.CompletedTasks = component.CompletedTasks.Where(u => u.ModifiedAt > lastExchange).ToList(),
            () => uploadData.Progresses =  component.Progresses.Where(u => u.ModifiedAt > lastExchange).ToList());
        
        return uploadData;
    }

    public async Task<bool> ProcessNewData(NewData newData)
    {
        await component.ClearTableAsync<User>();
        await component.ClearTableAsync<Models.Storage.Theme>();
        await component.ClearTableAsync<Models.Storage.Task>();
        await component.ClearTableAsync<Progress>();
        await component.ClearTableAsync<Models.Storage.Lesson>();
        await component.ClearTableAsync<CompletedTask>();

        await component.BulkInsertAsync(newData.Users, new BulkConfig { SetOutputIdentity = false });
        await component.BulkInsertAsync(newData.Themes, new BulkConfig { SetOutputIdentity = false });
        await component.BulkInsertAsync(newData.Tasks, new BulkConfig { SetOutputIdentity = false });
        await component.BulkInsertAsync(newData.Progresses, new BulkConfig { SetOutputIdentity = false });
        await component.BulkInsertAsync(newData.Lessons, new BulkConfig { SetOutputIdentity = false });
        await component.BulkInsertAsync(newData.CompletedTasks, new BulkConfig { SetOutputIdentity = false });
        
        return true;
    }
    
    public async Task<string?> Login(AuthModel login)
    {
        var user = component.Users.FirstOrDefault(u =>
            u.Username == login.Username &&
            u.Password == login.Password);

        if (user == null || user.IsBlocked)
            return null;

        user.LastLogin = DateTime.Now;
        user.ModifiedAt = DateTime.UtcNow;

        await component.Update(user);

        await SecureStorage.SetAsync("username", login.Username ?? "");
        await SecureStorage.SetAsync("password", login.Password ?? "");
        
        Preferences.Default.Set("user_id", user.Id);
        Preferences.Default.Set("username", user.Username);
        
        return "local_session";
    }

    public async Task<bool> Register(RegisterModel request, bool isSynced)
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
        };

        return await component.Insert(newUser);
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

    public async Task<ProfileInfo> GetProfileInfo(int userId)
    {
        var user = await component.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new Exception("Информация о пользователе не найдена");

        var themeStat = await GetStatisticForThemes(user.Id);

        return new ProfileInfo
        {
            LastName = user.LastName,
            FirstName = user.FirstName,
            Username = user.Username,
            ThemesStatistics = themeStat,
        };
    }

    private async Task<List<ThemesStatistic>> GetStatisticForThemes(int userId)
    {
        var progresses = await component.Progresses.ToListAsync();

        var tasks = await component.Tasks
            .Include(t => t.Theme)
            .ToListAsync();

        var completedTasks = await component.CompletedTasks
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.TaskId)
            .Select(g => g
                .OrderByDescending(x => x.CompletedAt)
                .First())
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
                    SolvedPercent = Math.Round(solvedPercent, 0),
                    SolvedCorrectPercent = Math.Round(correctPercent, 0),
                    ThemeId = g.Key.ThemeId,
                    ThemeName = g.Key.Title,
                    Level = progresses.FirstOrDefault(p => p.ThemeId == g.Key.ThemeId)?.Level ?? 1
                };
            });

        return statistic.Values.ToList();
    }

    public async Task<bool> SaveAnswers(int userId, List<UserAnswer> answers)
    {
        foreach (var answer in answers)
        {
            await SaveAnswer(userId, answer.TaskId, answer.IsCorrect);
        }

        return true;
    }

    public async Task<bool> SaveAnswer(int userId, int taskId, bool isCorrect)
    {
        var task = component.Tasks.FirstOrDefault(t => t.Id == taskId);

        if (task is null)
            throw new Exception($"Не найдено задание с id {taskId}");

        if (!component.Users.Any(u => u.Id == userId))
            throw new Exception($"Не найден пользователь с id {userId}");

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