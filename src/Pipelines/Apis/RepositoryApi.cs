using Microsoft.AspNetCore.Http.HttpResults;
using Pipelines.Extensions;
using Pipelines.Models.Repositories;
using Pipelines.Services;

public static class RepositoryApi
{
    public static RouteGroupBuilder MapRepositoryApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/");

        api.MapGet("/repositories/", List);
        api.MapPatch("/repositories/{id}", Patch);
        api.MapDelete("/repositories/{id}", Delete);

        return api;
    }

    private static async Task<Results<Ok<IEnumerable<RepositoryResponse>>, ProblemHttpResult>> List(
       RepositoryService service,
       IdentityService identityService,
       CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserIdentity();
        var result = await service.ListAsync(cancellationToken);

        return TypedResults.Ok(result);
    }
    private static async Task<Results<Ok, ProblemHttpResult>> Patch(
        Guid id,
        Patch<RepositoryRequest> patch,
        RepositoryService repositoryService,
        CancellationToken cancellationToken)
    {
        var result = await repositoryService.PatchAsync(id, patch, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Delete(
        Guid id,
        RepositoryService repositoryService,
        CancellationToken cancellationToken)
    {
        var result = await repositoryService.DeleteAsync(id, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        return TypedResults.Ok();
    }
}