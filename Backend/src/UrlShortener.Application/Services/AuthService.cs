using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity;

namespace UrlShortener.Application.Services;

public class AuthService(
    IIdentityService identityService,
    ICurrentUserService currentUser) : IAuthService
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

    public async Task<Result<UserResponseDto>> GetCurrentUserAsync()
    {
        var currentUserEmail = currentUser.Email;
        if (string.IsNullOrEmpty(currentUserEmail))
            return Result<UserResponseDto>.Failure("Current user email is not available");

        var currentUserId = currentUser.UserId;
        if (currentUserId == null)
            return Result<UserResponseDto>.Failure("Current user ID is not available");
        
        var currentUserRole = currentUser.Role;
        if (string.IsNullOrEmpty(currentUserRole))
            return Result<UserResponseDto>.Failure("Current user role is not available");
        
        var userDto = new UserResponseDto(
            currentUserId.Value,
            currentUserEmail,
            currentUserRole
        );
        
        return Result<UserResponseDto>.Success(userDto);
    }

    public async Task LogoutAsync()
    {
        await identityService.LogoutAsync();
    }
}
