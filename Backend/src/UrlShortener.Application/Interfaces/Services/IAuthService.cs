using UrlShortener.Application.Dto;
using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result> LoginAsync(LoginRequestDto loginRequest);

    Task<Result> RegisterAsync(LoginRequestDto loginRequest);

    Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersAsync();

    Task<Result> DeleteUserByIdAsync(int userId);
}
