using ErrorOr;

namespace Pipelines.Errors;

public static class UserErrors
{
    public static Error InvalidCredentials => Error.Validation(
        code: "User.InvalidCredentials",
        description: "Invalid email or password");

    public static Error UserNotFound => Error.NotFound(
        code: "User.NotFound",
        description: "User not found");

    public static Error AccountInactive => Error.Validation(
        code: "User.AccountInactive",
        description: "Account is inactive");

    public static Error AccountSuspended => Error.Validation(
        code: "User.AccountSuspended",
        description: "Account has been suspended");

    public static Error EmailNotVerified => Error.Validation(
        code: "User.EmailNotVerified",
        description: "Account not verified, please verify your email first");

    public static Error AccountStatusInvalid => Error.Validation(
        code: "User.AccountStatusInvalid",
        description: "Invalid account status");

    public static Error AccountLocked(DateTimeOffset lockoutEnd) => Error.Validation(
        code: "User.AccountLocked",
        description: $"Account is locked until {lockoutEnd:yyyy-MM-dd HH:mm:ss}");
}