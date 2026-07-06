namespace Pipelines.Core.Abstractions;

public interface ITaskDispatcher
{
    Task RunAsync(CancellationToken cancellationToken = default);
    Task CancelAsync(CancellationToken cancellationToken = default);
}