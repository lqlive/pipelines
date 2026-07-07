namespace Pipelines.Core.Models;

public sealed class TaskLogEntry
{
    public Guid TaskId { get; init; }
    public string StepName { get; init; } = string.Empty;
    public string Stream { get; init; } = "stdout";
    public string Text { get; init; } = string.Empty;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}