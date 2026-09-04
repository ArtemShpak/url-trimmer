using UrlShortener.Application.Dto;
using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<Result> RegistrationUserAsync(LoginRequestDto loginRequest);

    Task<Result> LoginAsync(LoginRequestDto loginRequest);

    Task LogoutAsync();

    Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersWithRolesAsync();

    Task<Result> DeleteUserByIdAsync(int userId);
}
