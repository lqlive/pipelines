using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;

namespace Pipelines.Core.Clients;

/// <summary>
/// Client interface for communicating with the Scheduler service
/// </summary>
public interface ISchedulerClient
{
    /// <summary>
    /// Submit a build job to the scheduler
    /// </summary>
    Task<bool> ScheduleJobAsync(Build build, JobPriority priority = JobPriority.Normal, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancel a scheduled or running job
    /// </summary>
    Task<bool> CancelJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get queue statistics from the scheduler
    /// </summary>
    Task<QueueStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get assignable jobs (for monitoring)
    /// </summary>
    Task<IEnumerable<Build>> GetAssignableJobsAsync(CancellationToken cancellationToken = default);
}
