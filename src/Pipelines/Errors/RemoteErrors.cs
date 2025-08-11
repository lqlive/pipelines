using ErrorOr;

namespace Pipelines.Errors;

public static class RemoteErrors
{
    public static Error Unauthorized => Error.Unauthorized(
      code: "Remote.InvalidCredentials",
      description: "Invalid credentials");
}
