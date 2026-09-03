using UrlShortener.Application.Interfaces.Repository;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Services;

public class ShortUrlService(
    IShortUrlRepository repository,
    IUrlShortenerService shortenerService,
    ICurrentUserService currentUserService) : IShortUrlService
{
    public async Task<(bool IsSuccess, string ErrorMessage, ShortUrl Result)> CreateShortUrlAsync(string originalUrl, int userId)
    {
        if (await repository.UrlExistsAsync(originalUrl))
        {
            return (false, "Такий URL вже існує в системі.", null);
        }

        string shortCode;
        do
        {
            shortCode = shortenerService.GenerateShortCode();
        } while (await repository.ShortCodeExistsAsync(shortCode));

        var newUrl = new ShortUrl
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        await repository.AddAsync(newUrl);
        await repository.SaveChangesAsync();

        return (true, string.Empty, newUrl);
    }

    public async Task<string> GetOriginalUrlAndRecordClickAsync(string shortCode)
    {
        var urlEntity = await repository.GetByShortCodeAsync(shortCode);

        if (urlEntity == null)
            return null;

        urlEntity.ClickCount++;

        await repository.UpdateAsync(urlEntity);
        await repository.SaveChangesAsync();

        return urlEntity.OriginalUrl;
    }

    public async Task<IEnumerable<ShortUrl>> GetAllUrlsAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<Result> DeleteShortUrlAsync(int id)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId is null) return Result.Failure("User in not authenticated.");

        var url = await repository.GetByIdAsync(id);
        if (url is null) return Result.Failure("URL not found.");

        bool isOwner = url.CreatedByUserId == currentUserId.Value;
        bool isAdmin = currentUserService.IsAdmin;

        if (!isOwner && !isAdmin) return Result.Failure("You do not have permission to delete this URL.");

        await repository.DeleteAsync(id);
        await repository.SaveChangesAsync();

        return Result.Success();
    }
}
