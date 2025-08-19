using Pipelines.Core.Entities.Repositories;

namespace Pipelines.Core.Entities.Builds;

public class Build
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public required Repository Repository { get; set; }
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
    public List<Log> Logs { get; set; } = new();
}