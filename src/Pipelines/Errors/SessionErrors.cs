using ErrorOr;

namespace Pipelines.Errors;

public static class SessionErrors
{
    public static Error SessionNotFound => Error.NotFound(
        code: "Session.NotFound",
        description: "Session not found or has expired");

    public static Error SessionExpired => Error.Validation(
        code: "Session.Expired", 
        description: "Session has expired");

    public static Error SessionTerminated => Error.Validation(
        code: "Session.Terminated",
        description: "Session has been terminated");

    public static Error SessionSuspended => Error.Validation(
        code: "Session.Suspended",
        description: "Session has been suspended");

    public static Error SessionRevoked => Error.Validation(
        code: "Session.Revoked",
        description: "Session has been revoked");

    public static Error InvalidSessionToken => Error.Validation(
        code: "Session.InvalidToken",
        description: "Invalid session token");

    public static Error SessionCreationFailed => Error.Failure(
        code: "Session.CreationFailed",
        description: "Failed to create session");

    public static Error SessionUpdateFailed => Error.Failure(
        code: "Session.UpdateFailed",
        description: "Failed to update session");

    public static Error TooManySessions => Error.Validation(
        code: "Session.TooMany",
        description: "Too many active sessions for this user");

    public static Error SessionInactive => Error.Validation(
        code: "Session.Inactive",
        description: "Session is not active");

    public static Error ConcurrentSessionLimit => Error.Validation(
        code: "Session.ConcurrentLimit",
        description: "Maximum concurrent sessions reached");
}