namespace Pipelines.Core.Runner;

public interface IJobServerQueue
{
    Task EnqueueAsync(object job, CancellationToken cancellationToken = default);
    Task<T?> DequeueAsync<T>(CancellationToken cancellationToken = default) where T : class;
}
