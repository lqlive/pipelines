using Pipelines.Core.Entities.Builds;

namespace Pipelines.Core.Scheduling;

/// <summary>
/// Job scheduler that coordinates between queue and runners
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Submit a build job for scheduling
    /// </summary>
    Task<bool> ScheduleBuildAsync(Build build, JobPriority priority = JobPriority.Normal, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Request the next job for a specific runner
    /// </summary>
    Task<Build?> RequestJobAsync(string runnerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Report job completion
    /// </summary>
    Task ReportJobCompletionAsync(string runnerId, Guid buildId, BuildStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancel a scheduled or running job
    /// </summary>
    Task<bool> CancelJobAsync(Guid buildId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get queue statistics
    /// </summary>
    Task<QueueStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get jobs that can be assigned to runners
    /// </summary>
    Task<IEnumerable<Build>> GetAssignableJobsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Queue statistics
/// </summary>
public record QueueStatistics(
    int PendingJobs,
    int RunningJobs,
    int CompletedJobs,
    int FailedJobs,
    int CancelledJobs,
    int ActiveRunners,
    int IdleRunners,
    TimeSpan AverageWaitTime,
    TimeSpan AverageExecutionTime
);
