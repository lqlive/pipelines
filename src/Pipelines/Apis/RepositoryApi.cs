using Microsoft.AspNetCore.Http.HttpResults;
using Pipelines.Models.Repositories;
using Pipelines.Services;

public static class RepositoryApi
{
    public static RouteGroupBuilder MapRepositoryApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/");

        api.MapGet("/repositories/", List);
        
        return api;
    }

    private static  async Task<Results<Ok<IEnumerable<RepositoryResponse>>, ProblemHttpResult>> List(
       RepositoryService service,
       IdentityService identityService,
       CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserIdentity();
        var result = await service.ListAsync(cancellationToken);

        return TypedResults.Ok(result);
    }
}