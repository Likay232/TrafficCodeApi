using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace MauiApp.Infrastructure.Models.Сomponents;

public class BaseComponent
{
    public async Task<bool> Insert<T>(T entityItem) where T : class
    {
        try
        {
            await using var context = new AppDbContext();
            await context.AddAsync(entityItem);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<bool> BulkInsertAsync<T>(List<T> entityItems, BulkConfig? bulkConfig = null, bool setSequence = false) 
        where T : class
    {
        try
        {
            await using var context = new AppDbContext();

            await context.BulkInsertAsync(entityItems, bulkConfig);

            if (setSequence)
            {
                var entityType = context.Model.FindEntityType(typeof(T))!;
                var tableName = entityType.GetTableName();
                var keyProperty = entityType.FindPrimaryKey()!.Properties.First();

                var maxId = await context.Set<T>()
                    .OrderByDescending(e => EF.Property<int>(e, keyProperty.Name))
                    .Select(e => EF.Property<int>(e, keyProperty.Name))
                    .FirstOrDefaultAsync();

                await context.Database.ExecuteSqlAsync(
                    $"UPDATE sqlite_sequence SET seq = {maxId} WHERE name = '{tableName}';"
                );
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
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
    
    public async Task<bool> BulkUpdateAsync<T>(List<T> entities) where T : class
    {
        try
        {
            await using var context = new AppDbContext();
            await context.BulkUpdateAsync(entities);
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

    public async Task<bool> BulkDeleteAsync<T>(List<T> entities) where T : class
    {
        try
        {
            await using var context = new AppDbContext();
            await context.BulkDeleteAsync(entities);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task ClearTableAsync<T>() where T : class
    {
        using (var context = new AppDbContext())
        {
            var tableName = context.Model.FindEntityType(typeof(T))!.GetTableName();
            
            var dbSet = context.Set<T>();
            await dbSet.ExecuteDeleteAsync();
            
            await context.Database.ExecuteSqlAsync($"DELETE FROM sqlite_sequence WHERE name='{tableName}';");
        }
    }
}