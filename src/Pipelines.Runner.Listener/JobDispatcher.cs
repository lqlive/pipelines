using Pipelines.Core.Models;

namespace Pipelines.Runner.Listener;

public class JobDispatcher : IJobDispatcher
{
    public Task RunAsync(JobRequest request, bool runOnce, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public Task<bool> CancelAsync(JobCancelRequest request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}