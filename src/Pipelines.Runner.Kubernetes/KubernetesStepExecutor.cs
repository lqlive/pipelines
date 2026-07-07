using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Kubernetes;

public sealed class KubernetesStepExecutor : IStepExecutor
{
    private readonly IKubernetesManager _manager;

    public KubernetesStepExecutor(IKubernetesManager manager)
    {
        _manager = manager;
    }

    public async Task<StepResult> ExecuteAsync(
        StepExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}