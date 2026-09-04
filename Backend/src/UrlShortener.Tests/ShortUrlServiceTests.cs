using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Repository;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Application.Services;
using UrlShortener.Core.Entity;
using UrlShortener.Core.Entity.Error;

namespace UrlShortener.Application.Tests;

public class ShortUrlServiceTests
{
    [Fact]
    public async Task CreateShortUrlAsync_WhenUserIsNotAuthenticated_ReturnsFailure()
    {
        var service = CreateSut(new StubShortUrlRepository(), new StubShortenerService(), new StubCurrentUserService());

        var result = await service.CreateShortUrlAsync("https://example.com");

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Auth.Unauthorized, result.Error);
    }

    [Fact]
    public async Task CreateShortUrlAsync_WhenUrlFormatIsInvalid_ReturnsFailure()
    {
        var service = CreateSut(new StubShortUrlRepository(), new StubShortenerService(), new StubCurrentUserService { UserId = 5, Email = "user@example.com", Role = "User" });

        var result = await service.CreateShortUrlAsync("not-a-valid-url");

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Url.InvalidFormat, result.Error);
    }

    [Fact]
    public async Task CreateShortUrlAsync_WhenOriginalUrlAlreadyExists_ReturnsFailure()
    {
        var repository = new StubShortUrlRepository();
        await repository.AddAsync(new ShortUrl
        {
            Id = 1,
            OriginalUrl = "https://example.com",
            ShortCode = "ABC123",
            CreatedByUserId = 5,
            CreatedDate = DateTime.UtcNow
        });

        var currentUser = new StubCurrentUserService { UserId = 5, Email = "user@example.com", Role = "User" };
        var service = CreateSut(repository, new StubShortenerService("GEN123"), currentUser);

        var result = await service.CreateShortUrlAsync("https://example.com");

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Url.AlreadyExists, result.Error);
    }

    [Fact]
    public async Task CreateShortUrlAsync_WhenInputIsValid_CreatesShortUrl()
    {
        var repository = new StubShortUrlRepository();
        var currentUser = new StubCurrentUserService { UserId = 5, Email = "user@example.com", Role = "User" };
        var shortener = new StubShortenerService("XYZ789");
        var service = CreateSut(repository, shortener, currentUser);

        var result = await service.CreateShortUrlAsync("https://example.com/article");

        Assert.True(result.IsSuccess);
        Assert.Equal("XYZ789", result.Value.ShortCode);
        Assert.Equal("https://example.com/article", result.Value.OriginalUrl);
        Assert.Equal(5, result.Value.CreatedByUserId);
        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task GetOriginalUrlAndRecordClickAsync_WhenShortCodeExists_ReturnsOriginalUrlAndIncrementsClicks()
    {
        var repository = new StubShortUrlRepository();
        var shortUrl = new ShortUrl
        {
            Id = 1,
            OriginalUrl = "https://example.com/page",
            ShortCode = "KLM456",
            CreatedByUserId = 9,
            CreatedDate = DateTime.UtcNow,
            ClickCount = 2,
        };
        await repository.AddAsync(shortUrl);

        var service = CreateSut(repository, new StubShortenerService(), new StubCurrentUserService());

        var originalUrl = await service.GetOriginalUrlAndRecordClickAsync("KLM456");

        Assert.Equal("https://example.com/page", originalUrl);
        Assert.Equal(3, repository.Items[1].ClickCount);
    }

    [Fact]
    public async Task GetOriginalUrlAndRecordClickAsync_WhenShortCodeDoesNotExist_ReturnsNull()
    {
        var service = CreateSut(new StubShortUrlRepository(), new StubShortenerService(), new StubCurrentUserService());

        var originalUrl = await service.GetOriginalUrlAndRecordClickAsync("NOPE");

        Assert.Null(originalUrl);
    }

    [Fact]
    public async Task DeleteShortUrlAsync_WhenUserIsNotAuthenticated_ReturnsFailure()
    {
        var service = CreateSut(new StubShortUrlRepository(), new StubShortenerService(), new StubCurrentUserService());

        var result = await service.DeleteShortUrlAsync(1);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Auth.Unauthorized, result.Error);
    }

    [Fact]
    public async Task DeleteShortUrlAsync_WhenUrlIsMissing_ReturnsFailure()
    {
        var currentUser = new StubCurrentUserService { UserId = 10, Email = "owner@example.com", Role = "User" };
        var service = CreateSut(new StubShortUrlRepository(), new StubShortenerService(), currentUser);

        var result = await service.DeleteShortUrlAsync(999);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Url.NotFound, result.Error);
    }

    [Fact]
    public async Task DeleteShortUrlAsync_WhenUserIsNotOwnerAndNotAdmin_ReturnsFailure()
    {
        var repository = new StubShortUrlRepository();
        await repository.AddAsync(new ShortUrl
        {
            Id = 1,
            OriginalUrl = "https://example.com",
            ShortCode = "OWNER1",
            CreatedByUserId = 8,
            CreatedDate = DateTime.UtcNow,
        });

        var currentUser = new StubCurrentUserService { UserId = 10, Email = "other@example.com", Role = "User" };
        var service = CreateSut(repository, new StubShortenerService(), currentUser);

        var result = await service.DeleteShortUrlAsync(1);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Auth.Forbidden, result.Error);
    }

    [Fact]
    public async Task DeleteShortUrlAsync_WhenUserOwnsUrl_DeletesItSuccessfully()
    {
        var repository = new StubShortUrlRepository();
        await repository.AddAsync(new ShortUrl
        {
            Id = 1,
            OriginalUrl = "https://example.com",
            ShortCode = "OWNED1",
            CreatedByUserId = 7,
            CreatedDate = DateTime.UtcNow,
        });

        var currentUser = new StubCurrentUserService { UserId = 7, Email = "owner@example.com", Role = "User" };
        var service = CreateSut(repository, new StubShortenerService(), currentUser);

        var result = await service.DeleteShortUrlAsync(1);

        Assert.True(result.IsSuccess);
        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task GetShortUrlDetailsAsync_WhenUrlExists_ReturnsDetailsWithCanModifyFlag()
    {
        var repository = new StubShortUrlRepository();
        var item = new ShortUrl
        {
            Id = 2,
            OriginalUrl = "https://example.com/news",
            ShortCode = "NEWS1",
            CreatedByUserId = 11,
            CreatedDate = new DateTime(2026, 9, 4, 12, 0, 0, DateTimeKind.Utc),
            ClickCount = 14
        };
        await repository.AddAsync(item);

        var currentUser = new StubCurrentUserService { UserId = 11, Email = "creator@example.com", Role = "User" };
        var service = CreateSut(repository, new StubShortenerService(), currentUser);

        var result = await service.GetShortUrlDetailsAsync(2);

        Assert.True(result.IsSuccess);
        Assert.Equal("NEWS1", result.Value.ShortCode);
        Assert.Equal("creator@example.com", result.Value.CreatedByUserName);
        Assert.True(result.Value.CanDelete);
    }

    [Fact]
    public async Task GetShortUrlDetailsAsync_WhenUrlDoesNotExist_ReturnsFailure()
    {
        var service = CreateSut(new StubShortUrlRepository(), new StubShortenerService(), new StubCurrentUserService());

        var result = await service.GetShortUrlDetailsAsync(5);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Url.NotFound, result.Error);
    }

    [Fact]
    public async Task GetAllUrlsAsync_ReturnsAllUrls()
    {
        var repository = new StubShortUrlRepository();
        await repository.AddAsync(new ShortUrl { Id = 1, OriginalUrl = "https://a.com", ShortCode = "A1", CreatedByUserId = 1, CreatedDate = DateTime.UtcNow });
        await repository.AddAsync(new ShortUrl { Id = 2, OriginalUrl = "https://b.com", ShortCode = "B2", CreatedByUserId = 2, CreatedDate = DateTime.UtcNow });

        var service = CreateSut(repository, new StubShortenerService(), new StubCurrentUserService());

        var result = await service.GetAllUrlsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void UrlShortenerService_GenerateShortCode_ReturnsSixCharactersAndOnlyAllowedSymbols()
    {
        var service = new UrlShortenerService();

        var code = service.GenerateShortCode();

        Assert.Equal(6, code.Length);
        Assert.All(code, ch => Assert.Contains(ch, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));
    }

    private static ShortUrlService CreateSut(IShortUrlRepository repository, IUrlShortenerService shortenerService, ICurrentUserService currentUserService)
    {
        return new ShortUrlService(repository, shortenerService, currentUserService);
    }

    private sealed class StubShortUrlRepository : IShortUrlRepository
    {
        public Dictionary<int, ShortUrl> Items { get; } = new();

        public Task AddAsync(ShortUrl shortUrl)
        {
            if (shortUrl.Id == 0)
            {
                shortUrl.Id = Items.Count == 0 ? 1 : Items.Keys.Max() + 1;
            }

            Items[shortUrl.Id] = shortUrl;
            return Task.CompletedTask;
        }

        public Task<ShortUrl?> GetByIdAsync(int id)
        {
            return Task.FromResult(Items.TryGetValue(id, out var item) ? item : null);
        }

        public Task<IEnumerable<ShortUrl>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<ShortUrl>>(Items.Values.ToList());
        }

        public Task<ShortUrl> GetByShortCodeAsync(string shortCode)
        {
            return Task.FromResult(Items.Values.FirstOrDefault(u => u.ShortCode == shortCode)!);
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            return Task.FromResult(Items.Values.Any(u => u.ShortCode == shortCode));
        }

        public Task UpdateAsync(ShortUrl shortUrl)
        {
            Items[shortUrl.Id] = shortUrl;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            Items.Remove(id);
            return Task.CompletedTask;
        }

        public Task<bool> UrlExistsAsync(string originalUrl)
        {
            return Task.FromResult(Items.Values.Any(u => u.OriginalUrl == originalUrl));
        }
    }

    private sealed class StubShortenerService : IUrlShortenerService
    {
        private readonly string _code;

        public StubShortenerService(string code = "TEST01")
        {
            _code = code;
        }

        public string GenerateShortCode()
        {
            return _code;
        }
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string Role { get; set; } = string.Empty;

        public UserResponseDto? GetCurrentUser()
        {
            if (UserId == 0 || string.IsNullOrEmpty(Email))
                return null;

            return new UserResponseDto(UserId, Email, Role);
        }
    }
}
