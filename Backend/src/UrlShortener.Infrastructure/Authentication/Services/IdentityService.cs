using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity;
using UrlShortener.Core.Entity.Error;
using UrlShortener.Infrastructure.Authentication.Entity;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.Authentication.Services;

public class IdentityService(
    UserManager<Person> userManager,
    ApplicationContext dbContext,
    SignInManager<Person> signInManager) : IIdentityService
{
    public async Task<Result> RegistrationUserAsync(LoginRequestDto request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null) return Result.Failure(DomainErrors.Auth.EmailAlreadyInUse);

        var user = new Person
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return Result.Failure(DomainErrors.Auth.RegistrationFailed);

        await userManager.AddToRoleAsync(user, "User");
        return Result.Success();
    }


    public async Task<Result> LoginAsync(LoginRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Result.Failure(DomainErrors.Auth.InvalidCredentials);
        }

        var result = await signInManager.PasswordSignInAsync(
            user.UserName!,
            request.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Result.Failure(DomainErrors.Auth.InvalidCredentials);
        }

        return Result.Success();
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersWithRolesAsync()
    {
        var usersWithRoles = await (
            from user in dbContext.Users.AsNoTracking()
            join userRole in dbContext.UserRoles on user.Id equals userRole.UserId into ur
            from userRole in ur.DefaultIfEmpty()
            join role in dbContext.Roles on userRole.RoleId equals role.Id into r
            from role in r.DefaultIfEmpty()
            select new UserResponseDto(
                user.Id,
                user.Email,
                role != null ? role.Name! : "User"
            )
        ).ToListAsync();

        return Result<IEnumerable<UserResponseDto>>.Success(usersWithRoles);
    }

    public Task<Result> DeleteUserByIdAsync(int userId)
    {
        var user = dbContext.Users.Find(userId);
        if (user is null)
            return Task.FromResult(Result.Failure(DomainErrors.User.UserNotFound));

        dbContext.Users.Remove(user);
        dbContext.SaveChanges();

        return Task.FromResult(Result.Success());
    }
}
