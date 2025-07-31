using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Stores;
using Pipelines.Errors;
using Pipelines.Models.Users;

namespace Pipelines.Services;

public class UserService(IUserStore userStore, IPasswordHasher<User> passwordHasher)
{
    public async Task<ErrorOr<UserResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var user = await userStore.GetByEmailAsync(loginRequest.Email, cancellationToken);
        if (user == null)
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

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            await HandleLoginFailedAsync(user, cancellationToken);
            return UserErrors.InvalidCredentials;
        }

        await HandleLoginSuccessAsync(user, "", cancellationToken);

        if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, loginRequest.Password);
            await userStore.UpdateAsync(user, cancellationToken);
        }

        var result = new UserResponse();
        return result;
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
            UserStatus.PendingVerification => UserErrors.EmailNotVerified,
            _ => UserErrors.AccountStatusInvalid
        };
    }
}