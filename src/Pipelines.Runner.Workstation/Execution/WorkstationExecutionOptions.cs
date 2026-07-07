namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationExecutionOptions
{
    public string WorkRoot { get; init; } = Path.Combine(".pipelines", "work");
    public bool Cleanup { get; init; } = true;

    public ISet<string> AllowedInheritedEnvironmentVariables { get; init; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "PATH",
            "Path",
            "SystemRoot",
            "WINDIR",
            "TEMP",
            "TMP",
            "HOME",
            "USERPROFILE"
        };
}
