using UrlShortener.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces.Repository;
using UrlShortener.Core.Entity;

namespace UrlShortener.Infrastructure.Persistence.Repository;

public class AboutRepository : IAboutRepository
{
    private readonly ApplicationContext _context;

    public AboutRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<AboutPageContent?> GetAsync()
    {
        return await _context.AboutPages.FirstOrDefaultAsync();
    }

    public async Task AddAsync(AboutPageContent aboutPage)
    {
        await _context.AboutPages.AddAsync(aboutPage);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
