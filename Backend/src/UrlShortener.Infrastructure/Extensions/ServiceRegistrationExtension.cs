using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Interfaces.Repository;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Infrastructure.Authentication.Entity;
using UrlShortener.Infrastructure.Authentication.Services;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Persistence.Repository;
using UrlShortener.Infrastructure.Persistence.Seeder;

namespace UrlShortener.Infrastructure.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<Person, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IShortUrlRepository, ShortUrlRepository>();
        services.AddScoped<IAboutRepository, AboutRepository>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();


        return services;
    }

    public static async Task UseInfrastructureDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();

        await AdminSeeder.SeedRolesAndAdminAsync(services, configuration);
    }
}
