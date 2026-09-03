using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Services;

public class AuthService(
    IIdentityService identityService) : IAuthService
{
    public async Task<Result> LoginAsync(LoginRequestDto loginRequest)
    {
        var succeeded = await identityService.LoginAsync(loginRequest);
        if (!succeeded)
            return Result.Failure("Invalid email or password");

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(LoginRequestDto loginRequest)
    {
        var succeeded = await identityService.RegistrationUserAsync(loginRequest);
        if (!succeeded)
            return Result.Failure("User with this email already exists or password is invalid");

        return Result.Success();
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersAsync()
    {
        var users = await identityService.GetAllUsersWithRolesAsync();
        return Result<IEnumerable<UserResponseDto>>.Success(users.Value);
    }

    public async Task<Result> DeleteUserByIdAsync(int userId)
    {
        var result = await identityService.DeleteUserByIdAsync(userId);
        if (!result.IsSuccess)
            return Result.Failure("Failed to delete user");
        return result;
    }
}
