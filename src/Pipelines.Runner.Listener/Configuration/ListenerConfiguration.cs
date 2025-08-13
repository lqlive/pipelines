namespace Pipelines.Runner.Listener.Configuration;

/// <summary>
/// Configuration for the Runner Listener service
/// </summary>
public class ListenerConfiguration
{
    public const string SectionName = "Listener";

    /// <summary>
    /// Port to listen on
    /// </summary>
    public int Port { get; set; } = 5170;

    /// <summary>
    /// Bind to all interfaces
    /// </summary>
    public bool BindToAll { get; set; } = true;

    /// <summary>
    /// Job queue settings
    /// </summary>
    public JobQueueSettings JobQueue { get; set; } = new();

    /// <summary>
    /// Runner management settings
    /// </summary>
    public RunnerManagementSettings RunnerManagement { get; set; } = new();

    /// <summary>
    /// Performance settings
    /// </summary>
    public PerformanceSettings Performance { get; set; } = new();
}

/// <summary>
/// Job queue configuration
/// </summary>
public class JobQueueSettings
{
    /// <summary>
    /// Maximum jobs in queue before rejecting new submissions
    /// </summary>
    public int MaxQueueSize { get; set; } = 1000;

    /// <summary>
    /// How long to keep completed job records (for status queries)
    /// </summary>
    public TimeSpan CompletedJobRetention { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Default job timeout if not specified
    /// </summary>
    public TimeSpan DefaultJobTimeout { get; set; } = TimeSpan.FromHours(6);
}

/// <summary>
/// Runner management configuration
/// </summary>
public class RunnerManagementSettings
{
    /// <summary>
    /// How often to cleanup stale runners
    /// </summary>
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Consider runner stale after this period without heartbeat
    /// </summary>
    public TimeSpan RunnerStaleTimeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Maximum number of concurrent jobs per runner (global limit)
    /// </summary>
    public int MaxConcurrentJobsPerRunner { get; set; } = 10;
}

/// <summary>
/// Performance and resource configuration
/// </summary>
public class PerformanceSettings
{
    /// <summary>
    /// Enable detailed performance metrics
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Log performance statistics interval
    /// </summary>
    public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromMinutes(5);
}
