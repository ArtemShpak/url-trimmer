using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity;
using UrlShortener.Core.Entity.Error;

namespace UrlShortener.Application.Services;

public class AuthService(
    IIdentityService identityService,
    ICurrentUserService currentUser) : IAuthService
{
    public async Task<Result> LoginAsync(LoginRequestDto loginRequest)
    {
        return await identityService.LoginAsync(loginRequest);
    }

    public async Task<Result> RegisterAsync(LoginRequestDto loginRequest)
    {
        return await identityService.RegistrationUserAsync(loginRequest);
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersAsync()
    {
        var users = await identityService.GetAllUsersWithRolesAsync();
        return Result<IEnumerable<UserResponseDto>>.Success(users.Value);
    }

    public async Task<Result> DeleteUserByIdAsync(int userId)
    {
        return await identityService.DeleteUserByIdAsync(userId);
    }

    public async Task<Result<UserResponseDto>> GetCurrentUserAsync()
    {
        var userDto = currentUser.GetCurrentUser();
    
        // Якщо ID нуль (не змогло розпарсити) або email порожній — користувач не авторизований
        if (userDto is null || userDto.Id == 0 || string.IsNullOrEmpty(userDto.Email))
        {
            return Result<UserResponseDto>.Failure(DomainErrors.Auth.Unauthorized);
        }

        return Result<UserResponseDto>.Success(userDto);
    }

    public async Task LogoutAsync()
    {
        await identityService.LogoutAsync();
    }
}
