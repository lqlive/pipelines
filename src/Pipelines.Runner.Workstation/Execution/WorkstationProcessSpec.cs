namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationProcessSpec
{
    public string StepName { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public IReadOnlyList<string> Arguments { get; init; } = [];
    public string WorkingDirectory { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> Environment { get; init; } =
        new Dictionary<string, string>();
    public int? TimeoutMinutes { get; init; }
}
