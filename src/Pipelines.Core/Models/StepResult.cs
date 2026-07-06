namespace Pipelines.Core.Models;

public sealed class StepResult
{
    public string StepName { get; init; } = string.Empty;
    public int ExitCode { get; init; }
    public StepStatus Status { get; init; }
    public TimeSpan Duration { get; init; }
}