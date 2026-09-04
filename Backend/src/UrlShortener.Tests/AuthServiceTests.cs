using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Application.Services;
using UrlShortener.Core.Entity;
using UrlShortener.Core.Entity.Error;

namespace UrlShortener.Application.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsSuccess()
    {
        var identityService = new StubIdentityService { LoginResult = Result.Success() };
        var currentUser = new StubCurrentUserService();
        var service = new AuthService(identityService, currentUser);

        var result = await service.LoginAsync(new LoginRequestDto("user@example.com", "Password123!"));

        Assert.True(result.IsSuccess);
        Assert.True(identityService.LoginCalled);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreInvalid_ReturnsFailure()
    {
        var identityService = new StubIdentityService { LoginResult = Result.Failure(DomainErrors.Auth.InvalidCredentials) };
        var service = new AuthService(identityService, new StubCurrentUserService());

        var result = await service.LoginAsync(new LoginRequestDto("user@example.com", "wrong-password"));

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Auth.InvalidCredentials, result.Error);
    }

    [Fact]
    public async Task RegisterAsync_WhenRegistrationSucceeds_ReturnsSuccess()
    {
        var identityService = new StubIdentityService { RegistrationResult = Result.Success() };
        var service = new AuthService(identityService, new StubCurrentUserService());

        var result = await service.RegisterAsync(new LoginRequestDto("new@example.com", "Password123!"));

        Assert.True(result.IsSuccess);
        Assert.True(identityService.RegistrationCalled);
    }

    [Fact]
    public async Task RegisterAsync_WhenRegistrationFails_ReturnsFailure()
    {
        var identityService = new StubIdentityService { RegistrationResult = Result.Failure(DomainErrors.User.UserAlreadyExists) };
        var service = new AuthService(identityService, new StubCurrentUserService());

        var result = await service.RegisterAsync(new LoginRequestDto("exists@example.com", "Password123!"));

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.UserAlreadyExists, result.Error);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedUsers()
    {
        var expectedUsers = new[]
        {
            new UserResponseDto(1, "admin@example.com", "Admin"),
            new UserResponseDto(2, "user@example.com", "User"),
        };

        var identityService = new StubIdentityService
        {
            UsersResult = Result<IEnumerable<UserResponseDto>>.Success(expectedUsers)
        };

        var service = new AuthService(identityService, new StubCurrentUserService());

        var result = await service.GetAllUsersAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUsers, result.Value);
    }

    [Fact]
    public async Task DeleteUserByIdAsync_WhenDeleteFails_ReturnsFailure()
    {
        var expectedError = new Error("User.DeleteFailed", "Delete failed");
        var identityService = new StubIdentityService
        {
            DeleteResult = Result.Failure(expectedError)
        };
        var service = new AuthService(identityService, new StubCurrentUserService());

        var result = await service.DeleteUserByIdAsync(42);

        Assert.True(result.IsFailure);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenCurrentUserInfoIsAvailable_ReturnsUserDto()
    {
        var currentUser = new StubCurrentUserService
        {
            Email = "current@example.com",
            UserId = 7,
            Role = "Admin"
        };
        var service = new AuthService(new StubIdentityService(), currentUser);

        var result = await service.GetCurrentUserAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Value.Id);
        Assert.Equal("current@example.com", result.Value.Email);
        Assert.Equal("Admin", result.Value.Role);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenCurrentUserDataIsMissing_ReturnsFailure()
    {
        var currentUser = new StubCurrentUserService
        {
            Email = string.Empty,
            UserId = 7,
            Role = "User"
        };
        var service = new AuthService(new StubIdentityService(), currentUser);

        var result = await service.GetCurrentUserAsync();

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Auth.Unauthorized, result.Error);
    }

    private sealed class StubIdentityService : IIdentityService
    {
        public Result LoginResult { get; set; } = Result.Success();
        public Result RegistrationResult { get; set; } = Result.Success();
        public bool LoginCalled { get; private set; }
        public bool RegistrationCalled { get; private set; }
        public Result<IEnumerable<UserResponseDto>> UsersResult { get; set; } = Result<IEnumerable<UserResponseDto>>.Success(Array.Empty<UserResponseDto>());
        public Result DeleteResult { get; set; } = Result.Success();

        public Task<Result> RegistrationUserAsync(LoginRequestDto loginRequest)
        {
            RegistrationCalled = true;
            return Task.FromResult(RegistrationResult);
        }

        public Task<Result> LoginAsync(LoginRequestDto loginRequest)
        {
            LoginCalled = true;
            return Task.FromResult(LoginResult);
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }

        public Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersWithRolesAsync()
        {
            return Task.FromResult(UsersResult);
        }

        public Task<Result> DeleteUserByIdAsync(int userId)
        {
            return Task.FromResult(DeleteResult);
        }
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string Role { get; set; } = string.Empty;

        public UserResponseDto? GetCurrentUser()
        {
            if (UserId == 0 || string.IsNullOrEmpty(Email))
                return null;

            return new UserResponseDto(UserId, Email, Role);
        }
    }
}