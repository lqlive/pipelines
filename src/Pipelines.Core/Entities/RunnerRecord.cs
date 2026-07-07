using Pipelines.Core.Models;

namespace Pipelines.Core.Entities;

public sealed class RunnerRecord
{
    public Guid RunnerId { get; init; }
    public RunnerProfile Profile { get; set; } = default!;
    public RunnerStatus Status { get; set; } = RunnerStatus.Online;
    public DateTimeOffset RegisteredAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastSeenAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset OnlineUntil { get; set; } = DateTimeOffset.UtcNow;
}
