using UrlShortener.Application.Interfaces.Repository;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Services;

public class AboutService : IAboutService
{
    private readonly IAboutRepository _repository;

    public AboutService(IAboutRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> GetContentAsync()
    {
        var aboutPage = await _repository.GetAsync();
        return aboutPage?.Content ?? "Алгоритм скорочення базується на генерації випадкових символів Base62.";
    }

    public async Task UpdateContentAsync(string content, int userId)
    {
        var aboutPage = await _repository.GetAsync();

        if (aboutPage == null)
        {
            aboutPage = new AboutPageContent();
            await _repository.AddAsync(aboutPage);
        }

        aboutPage.Content = content;
        aboutPage.LastModifiedByUserId = userId;
        aboutPage.LastModifiedDate = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }
}
