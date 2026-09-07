using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Interfaces.Repository;

public interface IShortUrlRepository
{
    Task<bool> UrlExistsAsync(string originalUrl);

    Task<bool> ShortCodeExistsAsync(string shortCode);

    Task AddAsync(ShortUrl shortUrl);

    Task<ShortUrl> GetByShortCodeAsync(string shortCode);

    Task UpdateAsync(ShortUrl shortUrl);

    Task SaveChangesAsync();

    Task<IEnumerable<ShortUrl>> GetAllAsync();

    Task<ShortUrl?> GetByIdAsync(int id);

    Task<string?> GetCreatedByUserEmailAsync(int userId);

    Task DeleteAsync(int id);
}
