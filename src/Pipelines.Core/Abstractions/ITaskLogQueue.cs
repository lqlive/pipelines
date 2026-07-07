using Pipelines.Core.Models;

namespace Pipelines.Core.Abstractions;

public interface ITaskLogQueue
{
    Task QueueAsync(
        TaskLogEntry entry,
        CancellationToken cancellationToken = default);
}