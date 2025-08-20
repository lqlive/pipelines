using Microsoft.AspNetCore.Http.HttpResults;

namespace Pipelines.Apis;

public static class SessionApi
{
    public static RouteGroupBuilder MapSessionApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/sessions");

        api.MapGet("/", List);
        return api;
    }
    private static async Task<Results<Ok, ProblemHttpResult>> List(
       CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return TypedResults.Ok();
    }
}
