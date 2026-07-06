using Pipelines.Core.Configuration;

namespace Pipelines.Runner.Docker.Container;

public interface IDockerManager
{
    Task PullImageAsync(string image, PullPolicy pullPolicy, CancellationToken cancellationToken = default);
    Task<string> CreateContainerAsync(DockerContainerSpec spec, CancellationToken cancellationToken = default);
    Task StartContainerAsync(string containerId, CancellationToken cancellationToken = default);
    Task<int> WaitContainerAsync(string containerId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<DockerLogEntry> StreamLogsAsync(string containerId, CancellationToken cancellationToken = default);
    Task StopContainerAsync(string containerId, TimeSpan timeout, CancellationToken cancellationToken = default);
    Task RemoveContainerAsync(string containerId, bool force = true, CancellationToken cancellationToken = default);
    Task<string> CreateNetworkAsync(string name, CancellationToken cancellationToken = default);
    Task RemoveNetworkAsync(string name, CancellationToken cancellationToken = default);
}
