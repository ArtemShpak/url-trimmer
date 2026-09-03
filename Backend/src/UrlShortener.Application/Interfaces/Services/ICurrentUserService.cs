namespace UrlShortener.Application.Interfaces.Services;

public interface ICurrentUserService
{
    int? UserId { get; }

    bool IsAdmin { get; }
}
