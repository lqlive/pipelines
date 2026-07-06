using Pipelines.Core.Configuration;

namespace Pipelines.Core.Models;

public sealed class StepExecutionContext
{
    public Guid TaskId { get; init; }
    public string WorkspacePath { get; init; } = string.Empty;
    public StepConfiguration Step { get; init; } = default!;
    public IReadOnlyDictionary<string, string> Environment { get; init; } =
        new Dictionary<string, string>();
}