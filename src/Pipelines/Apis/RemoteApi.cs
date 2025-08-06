using Microsoft.AspNetCore.Authentication;

namespace Pipelines.Apis;

public static class RemoteApi
{
    public static RouteGroupBuilder MapRemoteApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/remotes");

        api.MapGet("/{provider}/authorization/challenge", Challenge);
        api.MapGet("/{provider}/authorization/callback", Callback);

        return api;
    }
    private static IResult Challenge(string provider)
    {
      throw new NotImplementedException(provider);
    }
    private static Task<IResult> Callback(
       HttpContext context,
       CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}