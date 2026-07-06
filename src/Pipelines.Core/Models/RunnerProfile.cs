namespace Pipelines.Core.Models;

public sealed class RunnerProfile
{
    public Guid RunnerId { get; init; }
    public string Type { get; init; } = "docker";
    public string Os { get; init; } = string.Empty;
    public string Architecture { get; init; } = string.Empty;
    public int Capacity { get; init; } = 1;
    public IReadOnlyDictionary<string, string> Labels { get; init; } = new Dictionary<string, string>();
}