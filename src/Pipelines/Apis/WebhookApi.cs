using Microsoft.AspNetCore.Mvc;
using Pipelines.Abstractions;
using Pipelines.Core.Configuration;

namespace Pipelines.Apis;

public static class WebhookApi
{
    public static IEndpointRouteBuilder MapWebhooks(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/webhooks");
        group.MapPost("/{provider}", HandleAsync);
        return routes;
    }

    private static async Task<IResult> HandleAsync(
       string provider,
       [FromHeader(Name = "X-GitHub-Event")] string? githubEvent,
       [FromBody] WebhookDispatchRequest request,
       ITaskBroker broker,
       CancellationToken cancellationToken)
    {
        var eventName = githubEvent ?? string.Empty;
  
        var variables = new Dictionary<string, string>(request.Variables)
        {
            ["PIPELINES_TRIGGER"] = "webhook",
            ["PIPELINES_PROVIDER"] = provider,
            ["PIPELINES_EVENT"] = eventName
        };

        var taskId = await broker.EnqueueAsync(
            request.Pipeline,
            request.WorkspacePath,
            variables,
            cancellationToken);

        return TypedResults.Accepted($"/tasks/{taskId}");
    }

    private sealed record WebhookDispatchRequest(
        PipelineConfiguration Pipeline,
        string WorkspacePath,
        Dictionary<string, string> Variables);
}
