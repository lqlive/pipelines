using Pipelines.Core.Entities.Builds;

namespace Pipelines.Core.Scheduling;

/// <summary>
/// Job queue interface for managing build jobs
/// </summary>
public interface IJobQueue
{
    /// <summary>
    /// Enqueue a build job with optional priority
    /// </summary>
    Task EnqueueAsync(Build build, JobPriority priority = JobPriority.Normal, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dequeue the next available build job for a specific runner
    /// </summary>
    Task<Build?> DequeueAsync(string runnerId, string[] capabilities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get pending jobs count
    /// </summary>
    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get running jobs count
    /// </summary>
    Task<int> GetRunningCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Mark a job as completed
    /// </summary>
    Task CompleteJobAsync(Guid buildId, BuildStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancel a specific job
    /// </summary>
    Task<bool> CancelJobAsync(Guid buildId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get job status and runner info
    /// </summary>
    Task<JobStatus?> GetJobStatusAsync(Guid buildId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Job priority levels
/// </summary>
public enum JobPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}

/// <summary>
/// Job status information
/// </summary>
public record JobStatus(
    Guid BuildId,
    BuildStatus Status,
    string? RunnerId,
    DateTimeOffset QueuedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    JobPriority Priority
);
