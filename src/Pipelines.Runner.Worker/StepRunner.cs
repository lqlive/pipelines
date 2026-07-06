using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Worker;

public sealed class StepRunner : IStepRunner
{
    private readonly IStepExecutor _stepExecutor;

    public StepRunner(IStepExecutor stepExecutor)
    {
        _stepExecutor = stepExecutor;
    }

    public Task<StepResult> RunAsync(
        StepExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        return _stepExecutor.ExecuteAsync(context, cancellationToken);
    }
}