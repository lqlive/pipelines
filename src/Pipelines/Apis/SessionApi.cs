using Pipelines.Services;
using Pipelines.Session;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Pipelines.Apis;

public static class SessionApi
{
    public static RouteGroupBuilder MapSessionApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/sessions");

        api.MapGet("/", List);
        api.MapDelete("/{id}", Revoke);
        return api;
    }
    private static async Task<Results<Ok<IEnumerable<UserSession>>, ProblemHttpResult>> List(
       ISessionManager sessionManager,
       IdentityService identityService,
       CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserIdentity();
        var result = await sessionManager.ListSessionsAsync(Guid.Parse(userId));
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Revoke(
      ISessionManager sessionManager,
      IdentityService identityService,
      Guid id,
      CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserIdentity();
        var result = await sessionManager.RevokeSessionAsync(Guid.Parse(userId), id);
        return TypedResults.Ok();
    }
}