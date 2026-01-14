using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Models.Storage;
using Task = WebApi.Infrastructure.Models.Storage.Task;

namespace WebApi.Infrastructure.Components;

public class DatabaseContext : DbContext
{
    private readonly string _connectionString;
    
    public DatabaseContext()
    {
        _connectionString = "Server=localhost;Port=5434;User Id=postgres;Password=12345;Database=xamarinDb";
    }

    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>().ToTable("users");
    modelBuilder.Entity<Theme>().ToTable("themes");
    modelBuilder.Entity<TestTask>().ToTable("test_tasks");
    modelBuilder.Entity<Test>().ToTable("tests");
    modelBuilder.Entity<Task>().ToTable("tasks");
    modelBuilder.Entity<Progress>().ToTable("progresses");
    modelBuilder.Entity<Lesson>().ToTable("lessons");
    modelBuilder.Entity<CompletedTask>().ToTable("completed_tasks");
    modelBuilder.Entity<UserDevice>().ToTable("user_devices");
    modelBuilder.Entity<TestUser>().ToTable("test_users");
    
    modelBuilder.Entity<Theme>()
        .HasMany(t => t.Tasks)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);
    
    modelBuilder.Entity<Task>()
        .HasOne(t => t.Theme)
        .WithMany()
        .HasForeignKey(t => t.ThemeId)
        .OnDelete(DeleteBehavior.Cascade);
    
    modelBuilder.Entity<User>()
        .HasMany(u => u.Progresses)
        .WithOne(p => p.User)
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<User>()
        .HasMany(u => u.CompletedTasks)
        .WithOne(ct => ct.User)
        .HasForeignKey(ct => ct.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<User>()
        .HasMany(u => u.Devices)
        .WithOne(d => d.User)
        .HasForeignKey(d => d.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<User>()
        .HasMany(u => u.TestUsers)
        .WithOne(tu => tu.User)
        .HasForeignKey(tu => tu.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    
    modelBuilder.Entity<Theme>()
        .HasMany(t => t.Lessons)
        .WithOne(l => l.Theme)
        .HasForeignKey(l => l.ThemeId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Theme>()
        .HasMany(t => t.Tasks)
        .WithOne(task => task.Theme)
        .HasForeignKey(task => task.ThemeId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Theme>()
        .HasMany(t => t.Progresses)
        .WithOne(p => p.Theme)
        .HasForeignKey(p => p.ThemeId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Task>()
        .HasMany(t => t.CompletedTasks)
        .WithOne(ct => ct.Task)
        .HasForeignKey(ct => ct.TaskId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Task>()
        .HasMany(t => t.TestTasks)
        .WithOne(tt => tt.Task)
        .HasForeignKey(tt => tt.TaskId)
        .OnDelete(DeleteBehavior.Cascade);
}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Theme> Themes { get; set; }
    public DbSet<TestTask> TestTasks { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<CompletedTask> CompletedTasks { get; set; }
    public DbSet<Progress> Progresses { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Test> Tests { get; set; }
    
    public DbSet<UserDevice> UserDevices { get; set; }
    public DbSet<TestUser> TestUsers { get; set; }
}