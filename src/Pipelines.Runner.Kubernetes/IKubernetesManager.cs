
namespace Pipelines.Runner.Kubernetes;

public interface IKubernetesManager
{
    Task CreatePodAsync(
        KubernetesPodSpec spec,
        CancellationToken cancellationToken = default);

    Task WaitPodRunningAsync(
        string podName,
        CancellationToken cancellationToken = default);

    Task<int> WaitPodCompletedAsync(
        string podName,
        CancellationToken cancellationToken = default);

    Task StreamLogsAsync(
        string podName,
        Func<string, Task> onLine,
        CancellationToken cancellationToken = default);

    Task DeletePodAsync(
        string podName,
        CancellationToken cancellationToken = default);
}