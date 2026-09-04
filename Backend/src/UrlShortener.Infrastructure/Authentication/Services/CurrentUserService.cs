using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Services;

namespace UrlShortener.Infrastructure.Authentication.Services;

public class CurrentUserService(IHttpContextAccessor context) : ICurrentUserService
{
    private ClaimsPrincipal? User => context?.HttpContext?.User;

    public int UserId
    {
        get
        {
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim?.Value, out var id) ? id : 0;
        }
    }

    public string Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public bool IsAdmin => User?.IsInRole("Admin") ?? false;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

    public UserResponseDto? GetCurrentUser()
    {
        var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null || !int.TryParse(idClaim.Value, out var id) || id == 0)
        {
            return null;
        }

        var email = User?.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        return new UserResponseDto(
            Id: id,
            Email: email,
            Role: Role ?? "User"
        );
    }
}
