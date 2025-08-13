namespace Pipelines.Core.Runner;

public interface IJobServer
{
    Task AppendLogAsync(Guid buildId, Guid? stepId, string content, CancellationToken cancellationToken = default);
    Task<bool> IsCancellationRequestedAsync(Guid buildId, CancellationToken cancellationToken = default);
}
