namespace Pipelines.Core.Models;

public sealed class PipelineResult
{
    public Guid TaskId { get; init; }
    public PipelineStatus Status { get; init; }
    public IReadOnlyList<StepResult> Steps { get; init; } = Array.Empty<StepResult>();
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset FinishedAt { get; init; }
    public TimeSpan Duration => FinishedAt - StartedAt;
    public string? ErrorMessage { get; init; }
}