using System.Threading.Tasks;
using Grpc.Core;
using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;
using Pipelines.Proto;

namespace Pipelines.Runner.Listener.Services;

public class GrpcRunnerService : RunnerService.RunnerServiceBase
{
    private readonly IJobScheduler _scheduler;
    private readonly IRunnerRegistry _registry;

    public GrpcRunnerService(IJobScheduler scheduler, IRunnerRegistry registry)
    {
        _scheduler = scheduler;
        _registry = registry;
    }

    public override async Task<RegisterRunnerResponse> RegisterRunner(RegisterRunnerRequest request, ServerCallContext context)
    {
        var runner = new RunnerInfo(
            Id: Guid.NewGuid().ToString(),
            Name: request.Name,
            Capabilities: request.Capabilities.ToArray(),
            MaxConcurrentJobs: request.MaxConcurrentJobs,
            Status: RunnerStatus.Idle,
            RegisteredAt: DateTimeOffset.UtcNow,
            LastHeartbeat: DateTimeOffset.UtcNow,
            Version: string.IsNullOrEmpty(request.Version) ? "1.0.0" : request.Version,
            Platform: string.IsNullOrEmpty(request.Platform) ? Environment.OSVersion.Platform.ToString() : request.Platform,
            Labels: request.Labels.ToDictionary(kv => kv.Key, kv => kv.Value)
        );

        await _registry.RegisterRunnerAsync(runner, context.CancellationToken);
        return new RegisterRunnerResponse { RunnerId = runner.Id };
    }

    public override async Task<HeartbeatResponse> Heartbeat(HeartbeatRequest request, ServerCallContext context)
    {
        await _registry.HeartbeatAsync(request.RunnerId, context.CancellationToken);
        return new HeartbeatResponse();
    }

    public override async Task<JobAssignment> RequestJob(JobRequest request, ServerCallContext context)
    {
        var build = await _scheduler.RequestJobAsync(request.RunnerId, context.CancellationToken);
        if (build is null)
        {
            return new JobAssignment { HasJob = false };
        }

        var result = new JobAssignment
        {
            HasJob = true,
            Build = new Pipelines.Proto.Build
            {
                Id = build.Id.ToString(),
                Repository = build.RepositoryUrl ?? string.Empty,
                TimeoutSeconds = build.TimeoutSeconds ?? 0,
            }
        };

        foreach (var step in build.Steps)
        {
            result.Build.Steps.Add(new Pipelines.Proto.Step
            {
                Id = step.Id.ToString(),
                Image = step.Image ?? string.Empty,
                Script = step.Script ?? string.Empty,
            });
        }

        return result;
    }

    public override async Task<ReportResultResponse> ReportResult(ReportResultRequest request, ServerCallContext context)
    {
        var status = (BuildStatus)request.Status;
        if (!Guid.TryParse(request.BuildId, out var buildId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "invalid build id"));
        }
        await _scheduler.ReportJobCompletionAsync(request.RunnerId, buildId, status, context.CancellationToken);
        return new ReportResultResponse();
    }
}


