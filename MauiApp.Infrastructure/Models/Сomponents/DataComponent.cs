using MauiApp.Infrastructure.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Task = MauiApp.Infrastructure.Models.Storage.Task;

namespace MauiApp.Infrastructure.Models.Сomponents;

public class DataComponent
{
    public IQueryable<User> Users => new AppDbContext().Users.AsQueryable();
    public IQueryable<Lesson> Lessons => new AppDbContext().Lessons;
    public IQueryable<CompletedTask> CompletedTasks => new AppDbContext().CompletedTasks;
    public IQueryable<Progress> Progresses => new AppDbContext().Progresses;
    public IQueryable<Theme> Themes => new AppDbContext().Themes;
    public IQueryable<Task> Tasks => new AppDbContext().Tasks;
    
    public async Task<bool> Insert<T>(T entityItem) where T : class
    {
        try
        {
            await using var context = new AppDbContext();
            await context.AddAsync(entityItem);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> Update<T>(T entityItem) where T : class
    {
        try
        {
            await using var context = new AppDbContext();
            context.Entry(entityItem).State = EntityState.Modified;
            context.Update(entityItem);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Delete<T>(int entityId) where T : class
    {
        try
        {
            await using var context = new AppDbContext();
            var entity = await context.Set<T>().FindAsync(entityId);

            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}