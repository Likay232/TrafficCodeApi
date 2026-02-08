using EFCore.BulkExtensions;
using MauiApp.Infrastructure.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Task = MauiApp.Infrastructure.Models.Storage.Task;

namespace MauiApp.Infrastructure.Models.Сomponents;

public class DataComponent : BaseComponent
{
    public IQueryable<User> Users => new AppDbContext().Users.AsQueryable();
    public IQueryable<Lesson> Lessons => new AppDbContext().Lessons;
    public IQueryable<CompletedTask> CompletedTasks => new AppDbContext().CompletedTasks;
    public IQueryable<Progress> Progresses => new AppDbContext().Progresses;
    public IQueryable<Theme> Themes => new AppDbContext().Themes;
    public IQueryable<Task> Tasks => new AppDbContext().Tasks;
    public IQueryable<ExchangeHistory> ExchangeHistory => new AppDbContext().ExchangeHistory;
}