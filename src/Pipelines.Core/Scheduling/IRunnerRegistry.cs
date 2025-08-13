namespace Pipelines.Core.Scheduling;

/// <summary>
/// Runner registry for tracking available runners
/// </summary>
public interface IRunnerRegistry
{
    /// <summary>
    /// Register a new runner
    /// </summary>
    Task RegisterRunnerAsync(RunnerInfo runner, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update runner heartbeat
    /// </summary>
    Task HeartbeatAsync(string runnerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unregister a runner
    /// </summary>
    Task UnregisterRunnerAsync(string runnerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all active runners
    /// </summary>
    Task<IEnumerable<RunnerInfo>> GetActiveRunnersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get available runners that can handle specific capabilities
    /// </summary>
    Task<IEnumerable<RunnerInfo>> GetAvailableRunnersAsync(string[] requiredCapabilities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Mark runner as busy/idle
    /// </summary>
    Task SetRunnerStatusAsync(string runnerId, RunnerStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Clean up stale runners (no heartbeat)
    /// </summary>
    Task CleanupStaleRunnersAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
}

/// <summary>
/// Runner information
/// </summary>
public record RunnerInfo(
    string Id,
    string Name,
    string[] Capabilities,
    int MaxConcurrentJobs,
    RunnerStatus Status,
    DateTimeOffset RegisteredAt,
    DateTimeOffset LastHeartbeat,
    string Version,
    string Platform,
    Dictionary<string, string> Labels
);

/// <summary>
/// Runner status
/// </summary>
public enum RunnerStatus
{
    Idle = 0,
    Busy = 1,
    Offline = 2,
    Draining = 3  // Finishing current jobs but not taking new ones
}
