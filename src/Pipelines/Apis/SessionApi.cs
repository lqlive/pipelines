using Pipelines.Services;
using Pipelines.Session;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authentication;
using Pipelines.Models.Sessions;

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

    private static async Task<Results<Ok<IEnumerable<UserSessionResponse>>, ProblemHttpResult>> List(
       SessionService sessionService,
       IdentityService identityService,
       HttpContext httpContext,
       CancellationToken cancellationToken)
    {
        var sessionToken = await httpContext.GetTokenAsync("session_token");
        var userId = identityService.GetUserIdentity();

        var result = await sessionService.ListAsync(Guid.Parse(userId), sessionToken ?? string.Empty);
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