namespace Pipelines.Core.Entities.Builds;

public enum StepStatus
{
    Pending = 0,
    Running = 1,
    Succeeded = 2,
    Failed = 3,
    Skipped = 4
}

public class Step
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? Image { get; set; }
    public required string Script { get; set; }
    public StepStatus Status { get; set; } = StepStatus.Pending;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public int? ExitCode { get; set; }
    public int? TimeoutSeconds { get; set; }
}