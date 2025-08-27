namespace Pipelines.Runner.Docker.Container;
public interface IDockerManager
{
    Task<bool> CreateContainerAsync(IExecutionContext context, CancellationToken cancellationToken);
}