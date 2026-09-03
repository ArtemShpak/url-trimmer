using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Interfaces.Repository;

public interface IAboutRepository
{
    Task<AboutPageContent?> GetAsync();

    Task AddAsync(AboutPageContent aboutPage);

    Task SaveChangesAsync();
}
