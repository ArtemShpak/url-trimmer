using Microsoft.AspNetCore.Identity;
using UrlShortener.Core.Entity;

namespace UrlShortener.Infrastructure.Authentication.Entity;

public class Person : IdentityUser<int>
{
    public ICollection<ShortUrl> ShortUrls { get; set; } = [];

    public ICollection<AboutPageContent> EditedPages { get; set; } = [];
}
