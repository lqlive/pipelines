using Pipelines.Core.Models;

namespace Pipelines.Core.Abstractions;

public interface IStepExecutor
{
    Task<StepResult> ExecuteAsync(
        StepExecutionContext context,
        CancellationToken cancellationToken = default);
}