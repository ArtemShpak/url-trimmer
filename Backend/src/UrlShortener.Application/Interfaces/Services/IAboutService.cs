namespace UrlShortener.Application.Interfaces.Services;

public interface IAboutService
{
    Task<string> GetContentAsync();

    Task UpdateContentAsync(string content, int userId);
}
