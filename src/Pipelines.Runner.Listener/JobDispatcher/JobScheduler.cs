using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;
using Microsoft.Extensions.Logging;

namespace Pipelines.Runner.Listener.JobDispatcher;

/// <summary>
/// Job scheduler that coordinates between queue and runners
/// </summary>
public class JobScheduler : IJobScheduler
{
    private readonly IJobQueue _jobQueue;
    private readonly IRunnerRegistry _runnerRegistry;
    private readonly ILogger<JobScheduler> _logger;

    public JobScheduler(
        IJobQueue jobQueue,
        IRunnerRegistry runnerRegistry,
        ILogger<JobScheduler> logger)
    {
        _jobQueue = jobQueue;
        _runnerRegistry = runnerRegistry;
        _logger = logger;
    }

    public async Task<bool> ScheduleBuildAsync(Build build, JobPriority priority = JobPriority.Normal, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Scheduling build {BuildId} with priority {Priority}", build.Id, priority);
            
            await _jobQueue.EnqueueAsync(build, priority, cancellationToken);
            
            _logger.LogInformation("Build {BuildId} successfully queued", build.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule build {BuildId}", build.Id);
            return false;
        }
    }

    public async Task<Build?> RequestJobAsync(string runnerId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get runner capabilities
            var runners = await _runnerRegistry.GetActiveRunnersAsync(cancellationToken);
            var runner = runners.FirstOrDefault(r => r.Id == runnerId);
            
            if (runner == null)
            {
                _logger.LogWarning("Runner {RunnerId} not found or inactive", runnerId);
                return null;
            }

            // Try to get a job that matches runner capabilities
            var build = await _jobQueue.DequeueAsync(runnerId, runner.Capabilities, cancellationToken);
            
            if (build != null)
            {
                _logger.LogInformation("Assigned build {BuildId} to runner {RunnerId}", build.Id, runnerId);
                
                // Update runner status
                await _runnerRegistry.SetRunnerStatusAsync(runnerId, RunnerStatus.Busy, cancellationToken);
                
                // Increment job count
                if (_runnerRegistry is InMemoryRunnerRegistry memoryRegistry)
                {
                    memoryRegistry.IncrementJobCount(runnerId);
                }
            }

            return build;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting job for runner {RunnerId}", runnerId);
            return null;
        }
    }

    public async Task ReportJobCompletionAsync(string runnerId, Guid buildId, BuildStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Runner {RunnerId} completed build {BuildId} with status {Status}", 
                runnerId, buildId, status);

            // Mark job as completed in queue
            await _jobQueue.CompleteJobAsync(buildId, status, cancellationToken);

            // Update runner status and job count
            if (_runnerRegistry is InMemoryRunnerRegistry memoryRegistry)
            {
                memoryRegistry.DecrementJobCount(runnerId);
            }

            // Check if runner should go idle
            var runners = await _runnerRegistry.GetActiveRunnersAsync(cancellationToken);
            var runner = runners.FirstOrDefault(r => r.Id == runnerId);
            
            if (runner != null)
            {
                // If no more jobs or at capacity, mark as idle
                var availableRunners = await _runnerRegistry.GetAvailableRunnersAsync([], cancellationToken);
                var canTakeMore = availableRunners.Any(r => r.Id == runnerId);
                
                if (!canTakeMore)
                {
                    await _runnerRegistry.SetRunnerStatusAsync(runnerId, RunnerStatus.Idle, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting job completion for runner {RunnerId}, build {BuildId}", 
                runnerId, buildId);
        }
    }

    public async Task<bool> CancelJobAsync(Guid buildId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cancelling build {BuildId}", buildId);
            
            var result = await _jobQueue.CancelJobAsync(buildId, cancellationToken);
            
            if (result)
            {
                _logger.LogInformation("Build {BuildId} successfully cancelled", buildId);
            }
            else
            {
                _logger.LogWarning("Failed to cancel build {BuildId} - not found", buildId);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling build {BuildId}", buildId);
            return false;
        }
    }

    public async Task<QueueStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var pendingJobs = await _jobQueue.GetPendingCountAsync(cancellationToken);
            var runningJobs = await _jobQueue.GetRunningCountAsync(cancellationToken);
            var activeRunners = await _runnerRegistry.GetActiveRunnersAsync(cancellationToken);
            
            var activeRunnersList = activeRunners.ToList();
            var idleRunners = activeRunnersList.Count(r => r.Status == RunnerStatus.Idle);
            
            // TODO: Calculate averages from historical data
            var averageWaitTime = TimeSpan.Zero;
            var averageExecutionTime = TimeSpan.Zero;

            return new QueueStatistics(
                pendingJobs,
                runningJobs,
                0, // TODO: Track completed jobs
                0, // TODO: Track failed jobs
                0, // TODO: Track cancelled jobs
                activeRunnersList.Count,
                idleRunners,
                averageWaitTime,
                averageExecutionTime
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue statistics");
            return new QueueStatistics(0, 0, 0, 0, 0, 0, 0, TimeSpan.Zero, TimeSpan.Zero);
        }
    }

    public async Task<IEnumerable<Build>> GetAssignableJobsAsync(CancellationToken cancellationToken = default)
    {
        // This would return jobs from the queue that can be assigned
        // For now, we'll return an empty list as the actual implementation
        // would require peeking into the queue without dequeuing
        await Task.CompletedTask;
        return Enumerable.Empty<Build>();
    }
}
