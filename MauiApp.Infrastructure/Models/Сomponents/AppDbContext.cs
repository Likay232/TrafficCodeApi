using MauiApp.Infrastructure.Models.Storage;
using Microsoft.EntityFrameworkCore;
using Task = MauiApp.Infrastructure.Models.Storage.Task;

namespace MauiApp.Infrastructure.Models.Сomponents;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "app.db");
        
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Theme>().ToTable("themes");

        modelBuilder.Entity<Theme>()
            .Property(t => t.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<Task>()
            .Property(t => t.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Lesson>()
            .Property(t => t.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<Task>().ToTable("tasks");
        modelBuilder.Entity<Lesson>().ToTable("lessons");

        
        modelBuilder.Entity<Progress>().ToTable("progresses");
        modelBuilder.Entity<CompletedTask>().ToTable("completed_tasks");
        modelBuilder.Entity<ExchangeHistory>().ToTable("exchange_history");
    }
    
    public DbSet<Theme> Themes { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Progress> Progresses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<CompletedTask> CompletedTasks { get; set; }
    public DbSet<ExchangeHistory> ExchangeHistory { get; set; }
}