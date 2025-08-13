using Pipelines.Core.Entities.Builds;

namespace Pipelines.Runner.Docker.Worker;

public interface IWorker
{
    Task RunAsync(Build build, CancellationToken cancellationToken = default);
}


