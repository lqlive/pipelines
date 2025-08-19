namespace Pipelines.Core.Entities.Builds;

public class Step
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Image { get; set; }
    public required string Script { get; set; }
    public StepStatus Status { get; set; } = StepStatus.Pending;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public int? ExitCode { get; set; }
    public int? TimeoutSeconds { get; set; }
    public List<Log> Logs { get; set; } = new();
}