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

    public bool IsAdmin => User?.IsInRole("Admin") ?? false;
}
