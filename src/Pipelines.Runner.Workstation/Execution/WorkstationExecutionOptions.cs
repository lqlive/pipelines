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
            "COMSPEC",
            "ProgramFiles",
            "ProgramFiles(x86)",
            "ProgramW6432",
            "CommonProgramFiles",
            "CommonProgramFiles(x86)",
            "CommonProgramW6432",
            "ALLUSERSPROFILE",
            "ProgramData",
            "TEMP",
            "TMP",
            "HOME",
            "USERPROFILE"
        };
}
