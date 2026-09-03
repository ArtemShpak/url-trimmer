using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Interfaces.Repository;
using UrlShortener.Core.Entity;

namespace UrlShortener.Infrastructure.Persistence.Repository;

public class ShortUrlRepository : IShortUrlRepository
{
    private readonly ApplicationContext _context;

    public ShortUrlRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<bool> UrlExistsAsync(string originalUrl)
    {
        return await _context.ShortUrls.AnyAsync(s => s.OriginalUrl == originalUrl);
    }

    public async Task<bool> ShortCodeExistsAsync(string shortCode)
    {
        return await _context.ShortUrls.AnyAsync(s => s.ShortCode == shortCode);
    }

    public async Task AddAsync(ShortUrl shortUrl)
    {
        await _context.ShortUrls.AddAsync(shortUrl);
    }

    public async Task<ShortUrl> GetByShortCodeAsync(string shortCode)
    {
        return await _context.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    }

    public Task UpdateAsync(ShortUrl shortUrl)
    {
        _context.ShortUrls.Update(shortUrl);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ShortUrl>> GetAllAsync()
    {
        return await _context.ShortUrls.ToListAsync();
    }

    public async Task<ShortUrl?> GetByIdAsync(int id)
    {
        return await _context.ShortUrls.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task DeleteAsync(int id)
    {
        var url = await GetByIdAsync(id);
        if (url != null)
        {
            _context.ShortUrls.Remove(url);
        }
    }
}
