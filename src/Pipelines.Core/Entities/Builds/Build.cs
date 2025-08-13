namespace Pipelines.Core.Entities.Builds;

public enum BuildStatus
{
    Pending = 0,
    Running = 1,
    Succeeded = 2,
    Failed = 3,
    Canceled = 4
}

public class Build
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string RepositoryUrl { get; set; }
    public string? CommitRef { get; set; }
    public BuildStatus Status { get; set; } = BuildStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public int? TimeoutSeconds { get; set; }
    public string? ConcurrencyGroup { get; set; }
    public bool CancelInProgress { get; set; } = false;
    public bool CancellationRequested { get; set; } = false;
    public List<Step> Steps { get; set; } = new();
}