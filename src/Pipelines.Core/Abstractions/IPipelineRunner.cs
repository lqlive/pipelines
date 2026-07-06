using Pipelines.Core.Models;

namespace Pipelines.Core.Abstractions;

public interface IPipelineRunner
{
    Task<PipelineResult> RunAsync(
        PipelineExecutionContext context,
        CancellationToken cancellationToken = default);
}
