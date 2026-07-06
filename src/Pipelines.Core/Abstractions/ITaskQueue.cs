using Pipelines.Core.Models;

namespace Pipelines.Core.Abstractions;

public interface ITaskQueue
{
    Task<AcquiredTask?> TryAcquireAsync(
        RunnerProfile profile,
        CancellationToken cancellationToken = default);

    Task<bool> RenewLeaseAsync(
        Guid taskId, 
        Guid runnerId,
        string leaseToken,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        Guid taskId,
        Guid runnerId,
        string leaseToken,
        PipelineResult result,
        CancellationToken cancellationToken = default);
}
