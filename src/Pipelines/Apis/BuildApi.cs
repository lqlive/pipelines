using Microsoft.AspNetCore.Http.HttpResults;
using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;
using Pipelines.Services.Builds;

public static class BuildApi
{
    public static RouteGroupBuilder MapBuildApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/builds");

        api.MapPost("/submit", async (HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync<SubmitBuildRequest>();
            if (request?.Build is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            
            // TODO: Save build to database first
            
            var svc = context.RequestServices.GetRequiredService<BuildService>();
            var success = await svc.EnqueueAsync(request.Build, request.Priority, context.RequestAborted);
            
            if (success)
            {
                context.Response.StatusCode = StatusCodes.Status202Accepted;
                await context.Response.WriteAsJsonAsync(new { BuildId = request.Build.Id, Status = "Submitted" });
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        });

        api.MapPost("/{id:guid}/cancel", async (HttpContext context, Guid id) =>
        {
            var svc = context.RequestServices.GetRequiredService<BuildService>();
            var ok = await svc.CancelAsync(id, context.RequestAborted);
            context.Response.StatusCode = ok ? StatusCodes.Status202Accepted : StatusCodes.Status404NotFound;
        });

        api.MapPost("/{id:guid}/logs", async (HttpContext context, Guid id) =>
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync(context.RequestAborted);
            var stepIdText = context.Request.Query["stepId"].ToString();
            Guid? stepId = Guid.TryParse(stepIdText, out var s) ? s : null;
            var store = context.RequestServices.GetRequiredService<LogStorageService>();
            await store.AppendAsync(id, stepId, body, context.RequestAborted);
            context.Response.StatusCode = StatusCodes.Status204NoContent;
        });

        // Read logs with offset (for polling UI); bytes default 64KB
        api.MapGet("/{id:guid}/logs", async (HttpContext context, Guid id) =>
        {
            var stepIdText = context.Request.Query["stepId"].ToString();
            Guid? stepId = Guid.TryParse(stepIdText, out var s) ? s : null;
            long.TryParse(context.Request.Query["offset"], out var offset);
            int.TryParse(context.Request.Query["bytes"], out var bytes);
            bytes = bytes <= 0 ? 65536 : bytes;
            var store = context.RequestServices.GetRequiredService<LogStorageService>();
            var (data, next) = await store.ReadAsync(id, stepId, offset, bytes, context.RequestAborted);
            context.Response.Headers["x-next-offset"] = next.ToString();
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.Body.WriteAsync(data, 0, data.Length, context.RequestAborted);
        });

        // Get build details (from database)
        api.MapGet("/{id:guid}", async (HttpContext context, Guid id) =>
        {
            // TODO: Get build from database
            context.Response.StatusCode = StatusCodes.Status501NotImplemented;
            await context.Response.WriteAsync("Build database lookup not implemented yet");
        });

        // Get queue statistics (proxied from scheduler)
        api.MapGet("/queue/stats", async (HttpContext context) =>
        {
            var svc = context.RequestServices.GetRequiredService<BuildService>();
            var stats = await svc.GetStatisticsAsync(context.RequestAborted);
            await context.Response.WriteAsJsonAsync(stats);
        });

        return api;
    }
}

// Request/Response models
public record SubmitBuildRequest(Build Build, JobPriority Priority = JobPriority.Normal);

