namespace Pipelines.Runner.Workstation.Execution;

public static class WorkstationPathResolver
{
    public static string ResolveWorkspacePath(string workspacePath)
    {
        var resolved = string.IsNullOrWhiteSpace(workspacePath)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(workspacePath);

        Directory.CreateDirectory(resolved);
        return resolved;
    }

    public static string ResolveWorkingDirectory(
        string workspacePath,
        string? workingDirectory)
    {
        var workspaceFullPath = ResolveWorkspacePath(workspacePath);

        var resolved = string.IsNullOrWhiteSpace(workingDirectory)
            ? workspaceFullPath
            : Path.GetFullPath(Path.Combine(workspaceFullPath, workingDirectory));

        if (!IsWithinWorkspace(workspaceFullPath, resolved))
        {
            throw new InvalidOperationException(
                "Working directory must stay inside the workspace.");
        }

        Directory.CreateDirectory(resolved);
        return resolved;
    }

    private static bool IsWithinWorkspace(string workspacePath, string candidatePath)
    {
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var workspace = EnsureTrailingSeparator(Path.GetFullPath(workspacePath));
        var candidate = Path.GetFullPath(candidatePath);

        return candidate.Equals(
                workspace.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                comparison) ||
            candidate.StartsWith(workspace, comparison);
    }

    private static string EnsureTrailingSeparator(string path)
    {
        return path.EndsWith(Path.DirectorySeparatorChar) ||
            path.EndsWith(Path.AltDirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
    }
}
