using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Repository;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity;
using UrlShortener.Core.Entity.Error;

namespace UrlShortener.Application.Services;

public class ShortUrlService(
    IShortUrlRepository repository,
    IUrlShortenerService shortenerService,
    ICurrentUserService currentUserService) : IShortUrlService
{
    public async Task<Result<ShortUrl>> CreateShortUrlAsync(string originalUrl)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId == 0)
            return Result<ShortUrl>.Failure(DomainErrors.Auth.Unauthorized);

        if (await repository.UrlExistsAsync(originalUrl))
            return Result<ShortUrl>.Failure(DomainErrors.Url.AlreadyExists);

        string shortCode;
        do
        {
            shortCode = shortenerService.GenerateShortCode();
        } while (await repository.ShortCodeExistsAsync(shortCode));

        var newUrl = new ShortUrl
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            CreatedByUserId = currentUserId,
            CreatedDate = DateTime.UtcNow
        };

        await repository.AddAsync(newUrl);
        await repository.SaveChangesAsync();

        return Result<ShortUrl>.Success(newUrl);
    }

    public async Task<string?> GetOriginalUrlAndRecordClickAsync(string shortCode)
    {
        var urlEntity = await repository.GetByShortCodeAsync(shortCode);
        if (urlEntity is null)
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
        if (currentUserService.UserId == 0)
            return Result.Failure(DomainErrors.Auth.Unauthorized);

        var url = await repository.GetByIdAsync(id);
        if (url is null)
            return Result.Failure(DomainErrors.Url.NotFound);

        if (!CanModify(url))
            return Result.Failure(DomainErrors.Auth.Forbidden);

        await repository.DeleteAsync(id);
        await repository.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<ShortUrlDetailsResponse>> GetShortUrlDetailsAsync(int id)
    {
        var url = await repository.GetByIdAsync(id);
        if (url is null)
            return Result<ShortUrlDetailsResponse>.Failure(DomainErrors.Url.NotFound);

        var userName = string.IsNullOrEmpty(currentUserService.Email) 
            ? "Unknown" 
            : currentUserService.Email;

        var response = new ShortUrlDetailsResponse(
            url.Id,
            url.ShortCode,
            url.OriginalUrl,
            userName,
            url.CreatedDate,
            url.ClickCount,
            CanModify(url));

        return Result<ShortUrlDetailsResponse>.Success(response);
    }

    private bool CanModify(ShortUrl url)
    {
        var currentUserId = currentUserService.UserId;
        return currentUserService.IsAdmin ||
               (currentUserId != 0 && currentUserId == url.CreatedByUserId);
    }
}