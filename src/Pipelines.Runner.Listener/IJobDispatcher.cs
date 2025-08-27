using Pipelines.Core.Models;

namespace Pipelines.Runner.Listener;

public interface IJobDispatcher
{
    Task RunAsync(JobRequest request, bool runOnce, CancellationToken cancellationToken);
    Task<bool> CancelAsync(JobCancelRequest request,CancellationToken cancellation);
}
