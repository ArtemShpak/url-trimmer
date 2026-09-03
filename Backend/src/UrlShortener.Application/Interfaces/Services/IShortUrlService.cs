using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Interfaces.Services;

public interface IShortUrlService
{
    Task<(bool IsSuccess, string ErrorMessage, ShortUrl Result)> CreateShortUrlAsync(string originalUrl, int userId);

    Task<string> GetOriginalUrlAndRecordClickAsync(string shortCode);

    Task<IEnumerable<ShortUrl>> GetAllUrlsAsync();

    Task<Result> DeleteShortUrlAsync(int id);
}
