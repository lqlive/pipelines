using Docker.DotNet;
using Docker.DotNet.Models;

namespace Pipelines.Runner.Docker.Container;
public class DockerManager(DockerClient dockerClient) : IDockerManager
{
    public async Task<bool> CreateContainerAsync(IExecutionContext context, CancellationToken cancellationToken)
    {
        await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
        {
            Image = string.Empty,
        });

        return true;
    }
}
