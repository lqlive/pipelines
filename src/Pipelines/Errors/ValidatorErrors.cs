using ErrorOr;

namespace Pipelines.Errors;

public static class ValidatorErrors
{
    public static readonly Error SendFailed =
        Error.Failure("Verification.SendFailed", "Failed to send verification code");

    public static readonly Error CodeExpiredOrNotFound =
        Error.Validation("Verification.CodeExpiredOrNotFound",
        "Verification code has expired or not found. Please request a new code.");

    public static readonly Error CodeAlreadyUsed =
        Error.Validation("Verification.CodeAlreadyUsed",
        "This verification code has already been used.");

    public static readonly Error CodeExpired =
        Error.Validation("Verification.CodeExpired",
        "Verification code has expired. Please request a new code.");

    public static readonly Error TooManyAttempts =
        Error.Validation("Verification.TooManyAttempts",
        "Too many failed attempts. Please request a new verification code.");

    public static readonly Error VerificationFailed =
        Error.Failure("Verification.VerificationFailed",
        "Verification process failed due to an internal error.");

    public static Error InvalidCode(int remainingAttempts) =>
        Error.Validation("Verification.InvalidCode",
        $"Invalid verification code. You have {remainingAttempts} attempts remaining.");

    public static  Error ResendTooSoon(int remainingSeconds) =>
        Error.Validation("Verification.ResendTooSoon",
        $"Please wait {remainingSeconds} seconds before requesting a new code.");

    public static readonly Error ValueMismatch =
        Error.Validation("Validator.ValueMismatch", "The provided value does not match the cached value");

    public static readonly Error ValidationFailed =
        Error.Failure("Validator.ValidationFailed", "Validation process failed due to an internal error");
}