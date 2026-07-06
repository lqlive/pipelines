using Pipelines.Abstractions;
using Pipelines.Core.Configuration;
using Pipelines.Core.Models;

namespace Pipelines.Apis;

public static class TaskApi
{
    public static IEndpointRouteBuilder MapTasks(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/tasks");

        group.MapPost("/enqueue", async (
            EnqueueTaskRequest request,
            ITaskBroker broker,
            CancellationToken cancellationToken) =>
        {
            var taskId = await broker.EnqueueAsync(
                request.Pipeline,
                request.WorkspacePath,
                request.Variables,
                cancellationToken);

            return Results.Ok(new EnqueueTaskResponse(taskId));
        });

        group.MapPost("/acquire", async (
            RunnerProfile profile,
            ITaskBroker broker,
            CancellationToken cancellationToken) =>
        {
            var task = await broker.TryAcquireAsync(
                profile,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return task is null
                ? Results.NoContent()
                : Results.Ok(task);
        });

        group.MapPost("/{taskId:guid}/lease", async (
            Guid taskId,
            RenewLeaseRequest request,
            ITaskBroker broker,
            CancellationToken cancellationToken) =>
        {
            var renewed = await broker.RenewLeaseAsync(
                taskId,
                request.RunnerId,
                request.LeaseToken,
                TimeSpan.FromMilliseconds(request.LeaseDurationMilliseconds),
                cancellationToken);

            return renewed
                ? Results.NoContent()
                : Results.Conflict();
        });

        group.MapPost("/{taskId:guid}/complete", async (
            Guid taskId,
            CompleteTaskRequest request,
            ITaskBroker broker,
            CancellationToken cancellationToken) =>
        {
            var completed = await broker.CompleteAsync(
                taskId,
                request.RunnerId,
                request.LeaseToken,
                request.Result,
                cancellationToken);

            return completed
                ? Results.NoContent()
                : Results.Conflict();
        });

        return routes;
    }

    private sealed record EnqueueTaskRequest(
        PipelineConfiguration Pipeline,
        string WorkspacePath,
        Dictionary<string, string> Variables);

    private sealed record EnqueueTaskResponse(Guid TaskId);

    private sealed record RenewLeaseRequest(
        Guid RunnerId,
        string LeaseToken,
        long LeaseDurationMilliseconds);

    private sealed record CompleteTaskRequest(
        Guid RunnerId,
        string LeaseToken,
        PipelineResult Result);
}