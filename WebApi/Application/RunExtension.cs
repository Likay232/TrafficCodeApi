using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.AuthorizationRequirements.Handlers;
using WebApi.AuthorizationRequirements.Requirements;
using WebApi.Infrastructure.Components;
using WebApi.Services;

namespace WebApi.Application;

public static class RunExtension
{
    public static void ConnectionCreate(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;

        services.AddEndpointsApiExplorer();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowedOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => $"{type.Namespace}.{type.Name}");
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
        });

        services.AddTransient<DatabaseContext>(_ => new DatabaseContext(connectionString));
        services.AddScoped(_ => new DataComponent(connectionString));
    }

    public static void AddJwtAuthentication(this IServiceCollection services)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "super_secret_key_12345";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["AuthToken"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }

    public static void AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, NotBlockedHandler>();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("NotBlocked", policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new NotBlockedRequirement()));
        });
    }

    public static void MappingEndpoints(this WebApplication app)
    {
        app.MigrateDatabase();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("AllowedOrigins");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi");
            options.RoutePrefix = "swagger";
        });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=AuthMvc}/{action=Login}/{id?}");
        
        app.MapControllerRoute(
            name:"AdminRoute",
            pattern:"{controller=Admin}/{action=Index}/{id?}");
        
        app.MapGet("/", context =>
        {
            context.Response.Redirect("/AuthMvc/Login");
            return Task.CompletedTask;
        });

    }

    public static void RegistrationEndpoints(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AdminService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<ClientService>();
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
    }
    
    private static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<DatabaseContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}