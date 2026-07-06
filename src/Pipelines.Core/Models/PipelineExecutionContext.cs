using Pipelines.Core.Configuration;

namespace Pipelines.Core.Models;

public sealed class PipelineExecutionContext
{
    public Guid TaskId { get; init; }
    public Guid RunnerId { get; init; }
    public PipelineConfiguration Pipeline { get; init; } = default!;
    public string WorkspacePath { get; init; } = string.Empty;
    public Dictionary<string, string> Variables { get; init; } = new();
}
