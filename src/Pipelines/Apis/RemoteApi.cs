using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipelines.Core.Provider;
using Pipelines.Extensions;
using Pipelines.Services.Identity;
using Pipelines.Services.Remotes;

public static class RemoteApi
{
    public static RouteGroupBuilder MapRemoteApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/remotes");

        api.MapGet("/{provider}/authorization/challenge", Challenge);
        api.MapGet("/{provider}/authorization/callback", Callback);
        api.MapGet("/{provider}/repositories", List);

        return api;
    }
    private static async Task<IResult> Challenge(
        HttpContext context,
        string provider,
        RemoteService service,
        CancellationToken cancellationToken)
    {
        // Create AuthenticationProperties with minimal customization
        var properties = new AuthenticationProperties();
        var result = await service.GetChallengeUrlAsync(context, properties, cancellationToken);

        return TypedResults.Redirect(result);
    }

    private static async Task<IResult> Callback(
       HttpContext context,
       RemoteService service,
       string code,
       CancellationToken cancellationToken)
    {
        var properties = new AuthenticationProperties();
        var result = await service.CreateTicketAsync(code, properties, cancellationToken);
        return TypedResults.Json(result);
    }

    private static async Task<Results<Ok<RepositoryList>, ProblemHttpResult>> List(
       RemoteService service,
       IdentityService identityService,
       CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserIdentity();
        var result = await service.ListAsync(Guid.Parse(userId), cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        return TypedResults.Ok(result.Value);
    }
}