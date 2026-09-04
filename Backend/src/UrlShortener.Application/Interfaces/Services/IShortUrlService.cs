using UrlShortener.Application.Dto;
using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Interfaces.Services;

public interface IShortUrlService
{
    Task<Result<ShortUrl>> CreateShortUrlAsync(string originalUrl);

    Task<string?> GetOriginalUrlAndRecordClickAsync(string shortCode);

    Task<IEnumerable<ShortUrl>> GetAllUrlsAsync();

    Task<Result> DeleteShortUrlAsync(int id);

    Task<Result<ShortUrlDetailsResponse>> GetShortUrlDetailsAsync(int id);
    
}
