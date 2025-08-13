using Microsoft.AspNetCore.Http.HttpResults;
using Pipelines.Core.Provider;
using Pipelines.Extensions;
using Pipelines.Services.Identity;
using Pipelines.Services.Remotes;

public static class RepositoryApi
{
    public static RouteGroupBuilder MapRepositoryApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/");

        api.MapGet("/repositories/", List);
        
        return api;
    }

    private static   async Task<Results<Ok<RepositoryList>, ProblemHttpResult>> List(
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