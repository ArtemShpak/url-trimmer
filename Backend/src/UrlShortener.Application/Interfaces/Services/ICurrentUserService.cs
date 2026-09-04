namespace UrlShortener.Application.Interfaces.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    
    string Email { get; }

    bool IsAdmin { get; }
    
    string Role { get; }
}
