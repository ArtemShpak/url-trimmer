using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Application.Services;

namespace UrlShortener.Application.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IShortUrlService, ShortUrlService>();
        services.AddScoped<IUrlShortenerService, UrlShortenerService>();
        services.AddScoped<IAboutService, AboutService>();
        return services;
    }
}
