using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Models.Storage;
using MauiApp.Infrastructure.Models.Сomponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MauiApp.Infrastructure.Services;

public class LocalDataService(DataComponent component)
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
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, login.Username ?? "")
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

    public async System.Threading.Tasks.Task SaveAnswers(int userId, List<UserAnswer> answers)
    {
        foreach (var answer in answers)
        {
            await SaveAnswer(userId, answer.TaskId, answer.IsCorrect);
        }
    }

    public async System.Threading.Tasks.Task SaveAnswer(int userId, int taskId, bool isCorrect)
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
            CompletedAt = DateTime.UtcNow
        };

        await ChangeUserProgress(task, userId, isCorrect);
        await AlignDifficultyLevels();


        await component.Insert(completedTask);
    }

    private async System.Threading.Tasks.Task AlignDifficultyLevels()
    {
        var tasks = component.Tasks.ToList();

        var completedTasksAmount = component.CompletedTasks
            .GroupBy(ct => ct.TaskId)
            .ToDictionary(g => g.Key, g => g.Count());

        var correctCounts = component.CompletedTasks
            .Where(ct => ct.IsCorrect == true)
            .GroupBy(ct => ct.TaskId)
            .ToDictionary(g => g.Key, g => g.Count());

        var tasksToUpdate = new List<Models.Storage.Task>();

        foreach (var task in tasks)
        {
            if (!completedTasksAmount.TryGetValue(task.Id, out var totalCompleted))
                continue;

            var correctCount = correctCounts.TryGetValue(task.Id, out var count) ? count : 0;

            var percentage = totalCompleted > 0 ? (double)correctCount / totalCompleted * 100 : 0;

            int correctLevelForPercentage;
            switch (percentage)
            {
                case >= 0 and <= 20:
                    correctLevelForPercentage = 1;
                    break;
                case > 20 and <= 40:
                    correctLevelForPercentage = 2;
                    break;
                case > 40 and <= 60:
                    correctLevelForPercentage = 3;
                    break;
                case > 60 and <= 80:
                    correctLevelForPercentage = 4;
                    break;
                case > 80 and <= 100:
                    correctLevelForPercentage = 5;
                    break;
                default:
                    correctLevelForPercentage = 1;
                    break;
            }

            if (task.DifficultyLevel == correctLevelForPercentage) continue;

            task.DifficultyLevel = correctLevelForPercentage;
            tasksToUpdate.Add(task);
        }

        await component.BulkUpdateAsync(tasksToUpdate);
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