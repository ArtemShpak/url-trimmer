using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UrlShortener.Application.Interfaces.Services;

namespace UrlShortener.Infrastructure.Authentication.Services;

public class CurrentUserService(IHttpContextAccessor context) : ICurrentUserService
{
    private ClaimsPrincipal? User => context?.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim?.Value, out var id) ? id : null;
        }
    }

    public string Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public bool IsAdmin => User?.IsInRole("Admin") ?? false;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;
}
