namespace Pipelines.Core.Entities.Builds;

/// <summary>
/// Represents a log entry for a build step execution
/// </summary>
public class Log
{
    /// <summary>
    /// Unique identifier for the log entry
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the build this log belongs to
    /// </summary>
    public Guid BuildId { get; set; }

    /// <summary>
    /// ID of the step this log belongs to (null for build-level logs)
    /// </summary>
    public Guid? StepId { get; set; }

    /// <summary>
    /// Log content/message
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// When this log entry was created
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Log level/severity
    /// </summary>
    public LogLevel Level { get; set; } = LogLevel.Info;

    /// <summary>
    /// Source of the log entry
    /// </summary>
    public LogSource Source { get; set; } = LogSource.Stdout;

    /// <summary>
    /// Line number within the step execution (for ordering)
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Optional section/category for grouping related logs
    /// </summary>
    public string? Section { get; set; }

    /// <summary>
    /// Optional additional metadata as JSON
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Navigation property to the associated build
    /// </summary>
    public Build? Build { get; set; }

    /// <summary>
    /// Navigation property to the associated step
    /// </summary>
    public Step? Step { get; set; }
}

/// <summary>
/// Log level enumeration
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Detailed information for debugging
    /// </summary>
    Debug = 0,

    /// <summary>
    /// General information about execution
    /// </summary>
    Info = 1,

    /// <summary>
    /// Warning messages that don't stop execution
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error messages that may cause step failure
    /// </summary>
    Error = 3,

    /// <summary>
    /// Critical errors that stop execution
    /// </summary>
    Critical = 4
}

/// <summary>
/// Source of the log entry
/// </summary>
public enum LogSource
{
    /// <summary>
    /// Standard output from the step command
    /// </summary>
    Stdout = 0,

    /// <summary>
    /// Standard error from the step command
    /// </summary>
    Stderr = 1,

    /// <summary>
    /// System-generated log (runner, scheduler, etc.)
    /// </summary>
    System = 2,

    /// <summary>
    /// Debug information from the pipeline system
    /// </summary>
    Debug = 3,

    /// <summary>
    /// User-generated log entries
    /// </summary>
    User = 4
}