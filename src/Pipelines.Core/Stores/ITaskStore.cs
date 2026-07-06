using Pipelines.Core.Entities;
using Pipelines.Core.Models;

namespace Pipelines.Core.Stores;

public interface ITaskStore
{
    Task EnqueueAsync(TaskRecord task, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskRecord>> GetPendingAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskRecord>> GetLeasedAsync(CancellationToken cancellationToken = default);

    Task<TaskRecord?> GetAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task UpdateAsync(TaskRecord task, CancellationToken cancellationToken = default);

    Task CompleteAsync(Guid taskId, PipelineResult result, CancellationToken cancellationToken = default);
}