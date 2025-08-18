using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

using Pipelines.Core.Provider;
using Pipelines.Extensions;
using Pipelines.Models.Remotes;
using Pipelines.Services;
using Pipelines.Services.Remotes;

public static class RemoteApi
{
    public static RouteGroupBuilder MapRemoteApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/remotes");

        api.MapGet("/{provider}/authorization/challenge", Challenge);
        api.MapGet("/{provider}/authorization/callback", Callback);
        api.MapGet("/{provider}/repositories", List);
        api.MapPost("/{provider}/repositories/enable", Enable);

        return api;
    }
    private static async Task<IResult> Challenge(
        HttpContext context,
        string provider,
        string redirectUri,
        RemoteService service,
        CancellationToken cancellationToken)
    {
        // Create AuthenticationProperties with minimal customization
        var properties = new AuthenticationProperties()
        {
            RedirectUri = $"/api/remotes/github/authorization/callback?redirectUri={redirectUri}",
        };

        var result = await service.GetChallengeUrlAsync(context, properties, cancellationToken);

        return TypedResults.Redirect(result);
    }

    private static async Task<IResult> Callback(
       HttpContext context,
       RemoteService service,
       IdentityService identityService,
       string code,
       string redirectUri,
       CancellationToken cancellationToken)
    {
        var properties = new AuthenticationProperties();
        var ticket = await service.CreateTicketAsync(code, properties, cancellationToken);
        var userId = identityService.GetUserIdentity();

        var accessToken = ticket.Properties?.GetTokenValue("access_token") ?? string.Empty;
        var result = await service.CallbackAsync(Guid.Parse(userId), accessToken, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        return TypedResults.Redirect(redirectUri);
    }

    private static async Task<Results<Ok<RepositoryListResponse>, ProblemHttpResult>> List(
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

    private static async Task<Results<Ok, ProblemHttpResult>> Enable(
     RemoteService service,
     IdentityService identityService,
     EnableRepositoryRequest request,
     CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserIdentity();
        var result = await service.EnableAsync(Guid.Parse(userId), request, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        return TypedResults.Ok();
    }
}