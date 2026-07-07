using Pipelines.Abstractions;
using Pipelines.Core.Models;

namespace Pipelines.Apis;

public static class RunnerApi
{
    private static readonly TimeSpan DefaultHeartbeatTimeout = TimeSpan.FromMinutes(2);

    public static IEndpointRouteBuilder MapRunners(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/runners");

        group.MapGet("/", async (
            IRunnerRegistry registry,
            CancellationToken cancellationToken) =>
        {
            var runners = await registry.ListAsync(cancellationToken);
            return Results.Ok(runners);
        });

        group.MapPost("/register", async (
            RunnerHeartbeatRequest request,
            IRunnerRegistry registry,
            CancellationToken cancellationToken) =>
        {
            var runner = await registry.RegisterAsync(
                request.Profile,
                GetHeartbeatTimeout(request),
                cancellationToken);

            return Results.Ok(runner);
        });

        group.MapPost("/{runnerId:guid}/heartbeat", async (
            Guid runnerId,
            RunnerHeartbeatRequest request,
            IRunnerRegistry registry,
            CancellationToken cancellationToken) =>
        {
            if (runnerId != request.Profile.RunnerId)
            {
                return Results.BadRequest("Runner id does not match the profile.");
            }

            var runner = await registry.HeartbeatAsync(
                request.Profile,
                GetHeartbeatTimeout(request),
                cancellationToken);

            return Results.Ok(runner);
        });

        group.MapPost("/{runnerId:guid}/offline", async (
            Guid runnerId,
            IRunnerRegistry registry,
            CancellationToken cancellationToken) =>
        {
            var updated = await registry.OfflineAsync(runnerId, cancellationToken);

            return updated
                ? Results.NoContent()
                : Results.NotFound();
        });

        return routes;
    }

    private static TimeSpan GetHeartbeatTimeout(RunnerHeartbeatRequest request)
    {
        return request.HeartbeatTimeoutMilliseconds is > 0
            ? TimeSpan.FromMilliseconds(request.HeartbeatTimeoutMilliseconds.Value)
            : DefaultHeartbeatTimeout;
    }

    private sealed record RunnerHeartbeatRequest(
        RunnerProfile Profile,
        long? HeartbeatTimeoutMilliseconds);
}
