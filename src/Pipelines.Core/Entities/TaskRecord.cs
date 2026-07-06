using Pipelines.Core.Configuration;
using Pipelines.Core.Models;

namespace Pipelines.Core.Entities;

public class TaskRecord
{
    public Guid TaskId { get; init; } = Guid.NewGuid();
    public TaskState State { get; set; } = TaskState.Pending;
    public Guid? RunnerId { get; set; }
    public string LeaseToken { get; set; } = string.Empty;
    public DateTimeOffset? LeaseExpiresAt { get; set; }
    public int Attempt { get; set; }
    public PipelineConfiguration Pipeline { get; init; } = default!;
    public string WorkspacePath { get; init; } = string.Empty;
    public Dictionary<string, string> Variables { get; init; } = new();
    public PipelineResult? Result { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
