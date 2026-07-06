using Pipelines.Core.Models;

namespace Pipelines.Core.Abstractions;

public interface IStepRunner
{
    Task<StepResult> RunAsync(
        StepExecutionContext context,
        CancellationToken cancellationToken = default);
}