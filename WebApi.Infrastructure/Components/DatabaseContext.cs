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