namespace Pipelines.Runner.Workstation.Execution;

public sealed record WorkstationCommand(
    string FileName,
    IReadOnlyList<string> Arguments);
