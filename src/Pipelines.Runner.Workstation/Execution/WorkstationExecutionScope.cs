using Pipelines.Core.Models;

namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationExecutionScope : IAsyncDisposable
{
    private readonly WorkstationExecutionOptions _options;

    private WorkstationExecutionScope(
        WorkstationExecutionOptions options,
        string rootPath,
        string homePath,
        string workspacePath,
        string scriptsPath)
    {
        _options = options;
        RootPath = rootPath;
        HomePath = homePath;
        WorkspacePath = workspacePath;
        ScriptsPath = scriptsPath;
    }

    public string RootPath { get; }
    public string HomePath { get; }
    public string WorkspacePath { get; }
    public string ScriptsPath { get; }

    public static Task<WorkstationExecutionScope> CreateAsync(
        StepExecutionContext context,
        WorkstationExecutionOptions options,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rootBase = Path.GetFullPath(options.WorkRoot);
        var rootPath = Path.Combine(rootBase, context.TaskId.ToString("N"));
        var homePath = Path.Combine(rootPath, "home");
        var workspacePath = string.IsNullOrWhiteSpace(context.WorkspacePath)
            ? Path.Combine(rootPath, "src")
            : WorkstationPathResolver.ResolveWorkspacePath(context.WorkspacePath);
        var scriptsPath = Path.Combine(rootPath, "opt");

        Directory.CreateDirectory(homePath);
        Directory.CreateDirectory(workspacePath);
        Directory.CreateDirectory(scriptsPath);

        var scope = new WorkstationExecutionScope(
            options,
            rootPath,
            homePath,
            workspacePath,
            scriptsPath);

        return Task.FromResult(scope);
    }

    public ValueTask DisposeAsync()
    {
        if (!_options.Cleanup)
        {
            return ValueTask.CompletedTask;
        }

        try
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }
        }
        catch
        {
            // Workspace cleanup is best-effort and should not hide the step result.
        }

        return ValueTask.CompletedTask;
    }
}
