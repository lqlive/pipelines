using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;
using Pipelines.Core.Clients;

namespace Pipelines.Services.Builds;

/// <summary>
/// Build service that communicates with the external Scheduler service
/// </summary>
public class BuildService
{
    private readonly ISchedulerClient _schedulerClient;
    private readonly LogStorageService _logStorage;

    public BuildService(ISchedulerClient schedulerClient, LogStorageService logStorage)
    {
        _schedulerClient = schedulerClient;
        _logStorage = logStorage;
    }

    /// <summary>
    /// Enqueue a build with optional priority
    /// </summary>
    public async Task<bool> EnqueueAsync(Build build, JobPriority priority = JobPriority.Normal, CancellationToken ct = default)
    {
        return await _schedulerClient.ScheduleJobAsync(build, priority, ct);
    }

    /// <summary>
    /// Cancel a build
    /// </summary>
    public async Task<bool> CancelAsync(Guid buildId, CancellationToken ct = default)
    {
        return await _schedulerClient.CancelJobAsync(buildId, ct);
    }

    /// <summary>
    /// Get queue statistics
    /// </summary>
    public async Task<QueueStatistics> GetStatisticsAsync(CancellationToken ct = default)
    {
        return await _schedulerClient.GetStatisticsAsync(ct);
    }

    /// <summary>
    /// Get assignable jobs
    /// </summary>
    public async Task<IEnumerable<Build>> GetAssignableJobsAsync(CancellationToken ct = default)
    {
        return await _schedulerClient.GetAssignableJobsAsync(ct);
    }

    /// <summary>
    /// Check if build is running (for backwards compatibility)
    /// </summary>
    public async Task<(bool IsRunning, Build? Build)> TryGetRunningAsync(Guid buildId)
    {
        // Note: This method is deprecated and should be removed
        // The API should get build status from the database instead
        var assignableJobs = await GetAssignableJobsAsync();
        var runningJob = assignableJobs.FirstOrDefault(j => j.Id == buildId && j.Status == BuildStatus.Running);
        
        return runningJob != null ? (true, runningJob) : (false, null);
    }
}

