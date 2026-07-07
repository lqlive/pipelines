using Pipelines.Core.Configuration;

namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationScriptWriter
{
    public async Task<WorkstationCommand> CreateCommandAsync(
        WorkstationExecutionScope scope,
        StepConfiguration step,
        CancellationToken cancellationToken = default)
    {
        if (step.Commands.Count > 0)
        {
            return await WriteScriptAsync(scope, step, cancellationToken);
        }

        if (step.Entrypoint is { Count: > 0 })
        {
            return new WorkstationCommand(
                step.Entrypoint[0],
                step.Entrypoint.Skip(1).Concat(step.Command ?? []).ToArray());
        }

        if (step.Command is { Count: > 0 })
        {
            return new WorkstationCommand(step.Command[0], step.Command.Skip(1).ToArray());
        }

        return await WriteScriptAsync(scope, step, cancellationToken);
    }

    private static async Task<WorkstationCommand> WriteScriptAsync(
        WorkstationExecutionScope scope,
        StepConfiguration step,
        CancellationToken cancellationToken)
    {
        var fileName = CreateScriptFileName(scope.ScriptsPath, step.Name);
        var contents = OperatingSystem.IsWindows()
            ? CreateWindowsScript(step.Commands)
            : CreatePosixScript(step.Commands);

        await File.WriteAllTextAsync(fileName, contents, cancellationToken);

        return OperatingSystem.IsWindows()
            ? new WorkstationCommand("cmd.exe", ["/d", "/s", "/c", fileName])
            : new WorkstationCommand("/bin/sh", [fileName]);
    }

    private static string CreateWindowsScript(IReadOnlyList<string> commands)
    {
        if (commands.Count == 0)
        {
            return "@echo off\r\nexit /b 0\r\n";
        }

        var lines = new List<string>
        {
            "@echo off",
            "setlocal EnableExtensions"
        };

        foreach (var command in commands)
        {
            lines.Add(command);
            lines.Add("if errorlevel 1 exit /b %errorlevel%");
        }

        return string.Join("\r\n", lines) + "\r\n";
    }

    private static string CreatePosixScript(IReadOnlyList<string> commands)
    {
        if (commands.Count == 0)
        {
            return "#!/bin/sh\nexit 0\n";
        }

        return "#!/bin/sh\nset -e\n" + string.Join('\n', commands) + "\n";
    }

    private static string CreateScriptFileName(string scriptsPath, string stepName)
    {
        var segment = string.Concat(stepName.Select(c =>
            char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '-')).Trim('-');

        if (string.IsNullOrWhiteSpace(segment))
        {
            segment = "step";
        }

        var extension = OperatingSystem.IsWindows() ? ".cmd" : ".sh";
        return Path.Combine(scriptsPath, segment + extension);
    }
}
