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

    public static readonly Error EmailAlreadyExists = Error.Conflict(
        code: "User.EmailAlreadyExists",
        description: "This email address is already registered");

    public static readonly Error RegistrationFailed = Error.Failure(
        code: "User.RegistrationFailed",
        description: "Registration failed, please try again later");

    public static Error AccountLocked(DateTimeOffset lockoutEnd) => Error.Validation(
        code: "User.AccountLocked",
        description: $"Account is locked until {lockoutEnd:yyyy-MM-dd HH:mm:ss}");

    public static readonly Error NotAuthenticated = Error.Unauthorized(
        code: "User.NotAuthenticated",
        description: "User is not authenticated");

    public static readonly Error IncompleteUserInformation = Error.Validation(
        code: "User.IncompleteUserInformation",
        description: "Unable to obtain required user information (email and name) from external provider");

    public static readonly Error InvalidUserClaims = Error.Unauthorized(
        code: "User.InvalidUserClaims",
        description: "User ID not found in claims");
}