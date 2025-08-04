using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Stores;
using Pipelines.Errors;
using Pipelines.Models.Users;

namespace Pipelines.Services.Users;

public class UserService(IUserStore userStore, IPasswordHasher<User> passwordHasher,ILogger<UserService> logger)
{
    public async Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await userStore.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return UserErrors.EmailAlreadyExists;
        }
      
        var user = CreateUserFromRequest(request);
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        try
        {
            await userStore.CreateAsync(user, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register user with email {Email}", request.Email);
            return UserErrors.RegistrationFailed;
        }
    }
    public async Task<ErrorOr<UserResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await userStore.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return UserErrors.InvalidCredentials;
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            return UserErrors.AccountLocked(user.LockoutEnd.Value);
        }

        var statusError = ValidateUserStatus(user.Status);
        if (statusError.IsError)
        {
            return statusError.FirstError;
        }

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            await HandleLoginFailedAsync(user, cancellationToken);
            return UserErrors.InvalidCredentials;
        }

        await HandleLoginSuccessAsync(user, ipAddress, cancellationToken);

        if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            await userStore.UpdateAsync(user, cancellationToken);
        }

        return MapToUser(user);
    }

    private async Task HandleLoginFailedAsync(User user, CancellationToken cancellationToken)
    {
        user.FailedLoginAttempts++;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        if (user.FailedLoginAttempts >= 5)
        {
            user.LockoutEnd = DateTimeOffset.UtcNow.AddHours(1);
        }

        await userStore.UpdateAsync(user, cancellationToken);
    }

    private async Task HandleLoginSuccessAsync(User user, string? ipAddress, CancellationToken cancellationToken)
    {
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginTime = DateTimeOffset.UtcNow;
        user.LastLoginIp = ipAddress;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await userStore.UpdateAsync(user, cancellationToken);
    }

    private ErrorOr<Success> ValidateUserStatus(UserStatus status)
    {
        return status switch
        {
            UserStatus.Active => Result.Success,
            UserStatus.Inactive => UserErrors.AccountInactive,
            UserStatus.Suspended => UserErrors.AccountSuspended,
            _ => UserErrors.AccountStatusInvalid
        };
    }
    
    private static User CreateUserFromRequest(RegisterRequest request)
    {
        return new User
        {
            Name = request.Name,
            Email = request.Email,
            Status = UserStatus.Active,
            Provider = UserProvider.None,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            FailedLoginAttempts = 0,
        };
    }

    private UserResponse MapToUser(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Avatar = user.Avatar,
            Status = user.Status,
            Provider = user.Provider
        };
    }
}