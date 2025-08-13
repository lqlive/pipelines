using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Runner;

namespace Pipelines.Runner.Docker.Worker;

public class JobRunner : IWorker
{
    private readonly StepRunner _stepRunner;
    private readonly IJobServer _jobServer;

    public JobRunner(StepRunner stepRunner, IJobServer jobServer)
    {
        _stepRunner = stepRunner;
        _jobServer = jobServer;
    }

    public async Task RunAsync(Build build, CancellationToken cancellationToken = default)
    {
        try
        {
            build.Status = BuildStatus.Running;
            build.StartedAt = DateTimeOffset.UtcNow;

            // Create timeout token if specified
            using var timeoutCts = build.TimeoutSeconds.HasValue 
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                : null;
            
            if (timeoutCts != null)
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(build.TimeoutSeconds!.Value));

            var effectiveToken = timeoutCts?.Token ?? cancellationToken;

            foreach (var step in build.Steps)
            {
                // Check for cancellation before each step
                var isCancelled = await _jobServer.IsCancellationRequestedAsync(build.Id, effectiveToken);
                if (isCancelled || effectiveToken.IsCancellationRequested)
                {
                    build.Status = BuildStatus.Canceled;
                    build.FinishedAt = DateTimeOffset.UtcNow;
                    return;
                }

                await _stepRunner.RunAsync(build, step, effectiveToken);
                
                if (step.Status == StepStatus.Failed)
                {
                    build.Status = BuildStatus.Failed;
                    build.FinishedAt = DateTimeOffset.UtcNow;
                    return;
                }
            }

            build.Status = BuildStatus.Succeeded;
            build.FinishedAt = DateTimeOffset.UtcNow;
        }
        catch (OperationCanceledException)
        {
            build.Status = BuildStatus.Canceled;
            build.FinishedAt = DateTimeOffset.UtcNow;
        }
        catch (Exception ex)
        {
            await _jobServer.AppendLogAsync(build.Id, null, $"Job failed with error: {ex.Message}", cancellationToken);
            build.Status = BuildStatus.Failed;
            build.FinishedAt = DateTimeOffset.UtcNow;
        }
    }
}


