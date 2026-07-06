using Pipelines.Core.Configuration;
using Pipelines.Core.Models;

namespace Pipelines.Abstractions;

public interface ITaskBroker
{
    Task<Guid> EnqueueAsync(
        PipelineConfiguration pipeline,
        string workspacePath,
        Dictionary<string, string> variables,
        CancellationToken cancellationToken = default);

    Task<AcquiredTask?> TryAcquireAsync(
        RunnerProfile profile,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default);

    Task<bool> RenewLeaseAsync(
        Guid taskId,
        Guid runnerId,
        string leaseToken,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default);

    Task<bool> CompleteAsync(
        Guid taskId,
        Guid runnerId,
        string leaseToken,
        PipelineResult result,
        CancellationToken cancellationToken = default);
}