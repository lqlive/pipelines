using Microsoft.AspNetCore.Http.HttpResults;
using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;

namespace Pipelines.Runner.Listener.Apis;

public static class SchedulerApi
{
    public static RouteGroupBuilder MapSchedulerApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/scheduler");

        // Submit a job for scheduling
        api.MapPost("/jobs", async (HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync<ScheduleJobRequest>();
            if (request?.Build is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var scheduler = context.RequestServices.GetRequiredService<IJobScheduler>();
            var success = await scheduler.ScheduleBuildAsync(request.Build, request.Priority, context.RequestAborted);
            
            if (success)
            {
                context.Response.StatusCode = StatusCodes.Status202Accepted;
                await context.Response.WriteAsJsonAsync(new { JobId = request.Build.Id });
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        });

        // Cancel a job
        api.MapDelete("/jobs/{jobId:guid}", async (HttpContext context, Guid jobId) =>
        {
            var scheduler = context.RequestServices.GetRequiredService<IJobScheduler>();
            var success = await scheduler.CancelJobAsync(jobId, context.RequestAborted);
            
            context.Response.StatusCode = success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { Success = success });
        });

        // Get queue statistics
        api.MapGet("/stats", async (HttpContext context) =>
        {
            var scheduler = context.RequestServices.GetRequiredService<IJobScheduler>();
            var stats = await scheduler.GetStatisticsAsync(context.RequestAborted);
            await context.Response.WriteAsJsonAsync(stats);
        });

        // Get assignable jobs (for monitoring)
        api.MapGet("/jobs/assignable", async (HttpContext context) =>
        {
            var scheduler = context.RequestServices.GetRequiredService<IJobScheduler>();
            var jobs = await scheduler.GetAssignableJobsAsync(context.RequestAborted);
            await context.Response.WriteAsJsonAsync(jobs);
        });

        return api;
    }
}

// Runner APIs - moved from main API
public static class RunnerApi
{
    public static RouteGroupBuilder MapRunnerApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/runners");

        // Register a new runner
        api.MapPost("/register", async (HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync<RegisterRunnerRequest>();
            if (request is null || string.IsNullOrEmpty(request.Name))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid runner registration data");
                return;
            }

            var runner = new RunnerInfo(
                Id: Guid.NewGuid().ToString(),
                Name: request.Name,
                Capabilities: request.Capabilities ?? ["docker"],
                MaxConcurrentJobs: request.MaxConcurrentJobs,
                Status: RunnerStatus.Idle,
                RegisteredAt: DateTimeOffset.UtcNow,
                LastHeartbeat: DateTimeOffset.UtcNow,
                Version: request.Version ?? "1.0.0",
                Platform: request.Platform ?? Environment.OSVersion.Platform.ToString(),
                Labels: request.Labels ?? new Dictionary<string, string>()
            );

            var registry = context.RequestServices.GetRequiredService<IRunnerRegistry>();
            await registry.RegisterRunnerAsync(runner, context.RequestAborted);

            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsJsonAsync(new { RunnerId = runner.Id });
        });

        // Send heartbeat
        api.MapPost("/{runnerId}/heartbeat", async (HttpContext context, string runnerId) =>
        {
            var registry = context.RequestServices.GetRequiredService<IRunnerRegistry>();
            await registry.HeartbeatAsync(runnerId, context.RequestAborted);
            context.Response.StatusCode = StatusCodes.Status204NoContent;
        });

        // Request a job
        api.MapPost("/{runnerId}/jobs/request", async (HttpContext context, string runnerId) =>
        {
            var scheduler = context.RequestServices.GetRequiredService<IJobScheduler>();
            var job = await scheduler.RequestJobAsync(runnerId, context.RequestAborted);
            
            if (job is null)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }
            
            await context.Response.WriteAsJsonAsync(job);
        });

        // Report job completion
        api.MapPost("/{runnerId}/jobs/{jobId:guid}/complete", async (HttpContext context, string runnerId, Guid jobId) =>
        {
            var request = await context.Request.ReadFromJsonAsync<CompleteJobRequest>();
            if (request is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var scheduler = context.RequestServices.GetRequiredService<IJobScheduler>();
            await scheduler.ReportJobCompletionAsync(runnerId, jobId, request.Status, context.RequestAborted);
            context.Response.StatusCode = StatusCodes.Status204NoContent;
        });

        // Unregister runner
        api.MapDelete("/{runnerId}", async (HttpContext context, string runnerId) =>
        {
            var registry = context.RequestServices.GetRequiredService<IRunnerRegistry>();
            await registry.UnregisterRunnerAsync(runnerId, context.RequestAborted);
            context.Response.StatusCode = StatusCodes.Status204NoContent;
        });

        // Update runner status
        api.MapPut("/{runnerId}/status", async (HttpContext context, string runnerId) =>
        {
            var request = await context.Request.ReadFromJsonAsync<UpdateRunnerStatusRequest>();
            if (request is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var registry = context.RequestServices.GetRequiredService<IRunnerRegistry>();
            await registry.SetRunnerStatusAsync(runnerId, request.Status, context.RequestAborted);
            context.Response.StatusCode = StatusCodes.Status204NoContent;
        });

        // Get all active runners
        api.MapGet("/", async (HttpContext context) =>
        {
            var registry = context.RequestServices.GetRequiredService<IRunnerRegistry>();
            var runners = await registry.GetActiveRunnersAsync(context.RequestAborted);
            await context.Response.WriteAsJsonAsync(runners);
        });

        // Get available runners
        api.MapGet("/available", async (HttpContext context) =>
        {
            var capabilities = context.Request.Query["capabilities"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            var registry = context.RequestServices.GetRequiredService<IRunnerRegistry>();
            var runners = await registry.GetAvailableRunnersAsync(capabilities, context.RequestAborted);
            await context.Response.WriteAsJsonAsync(runners);
        });

        return api;
    }
}

// Request models
public record ScheduleJobRequest(Build Build, JobPriority Priority = JobPriority.Normal);
public record RegisterRunnerRequest(
    string Name,
    string[]? Capabilities = null,
    int MaxConcurrentJobs = 1,
    string? Version = null,
    string? Platform = null,
    Dictionary<string, string>? Labels = null
);
public record UpdateRunnerStatusRequest(RunnerStatus Status);
public record CompleteJobRequest(BuildStatus Status);
