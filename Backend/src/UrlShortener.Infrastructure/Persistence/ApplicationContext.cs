using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Entity;
using UrlShortener.Infrastructure.Authentication.Entity;

namespace UrlShortener.Infrastructure.Persistence;

public sealed class ApplicationContext : IdentityDbContext<Person, IdentityRole<int>, int>
{
    public DbSet<ShortUrl> ShortUrls { get; set; }

    public DbSet<AboutPageContent> AboutPages { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShortUrl>(builder =>
        {
            builder.HasOne<Person>()
                .WithMany(p => p.ShortUrls)
                .HasForeignKey(s => s.CreatedByUserId)
                .IsRequired();
        });

        modelBuilder.Entity<AboutPageContent>(builder =>
        {
            builder.HasOne<Person>()
                .WithMany(p => p.EditedPages)
                .HasForeignKey(a => a.LastModifiedByUserId)
                .IsRequired();
        });
    }
}
