using System.Diagnostics;
using Pipelines.Core.Abstractions;
using Pipelines.Core.Configuration;
using Pipelines.Core.Models;
using Pipelines.Runner.Docker.Container;

namespace Pipelines.Runner.Docker;

public sealed class DockerStepExecutor : IStepExecutor
{
    private readonly IDockerManager _dockerManager;

    public DockerStepExecutor(IDockerManager dockerManager)
    {
        _dockerManager = dockerManager;
    }

    public async Task<StepResult> ExecuteAsync(
        StepExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var step = context.Step;
        var stopwatch = Stopwatch.StartNew();

        string? containerId = null;
        using var timeoutCts = CreateTimeoutTokenSource(step, cancellationToken);
        var executionToken = timeoutCts?.Token ?? cancellationToken;

        try
        {
            await _dockerManager.PullImageAsync(step.Image, step.Pull, executionToken);

            var spec = CreateContainerSpec(context);
            containerId = await _dockerManager.CreateContainerAsync(spec, executionToken);

            await _dockerManager.StartContainerAsync(containerId, executionToken);

            var logTask = StreamLogsAsync(containerId, step.Name, executionToken);
            var exitCode = await _dockerManager.WaitContainerAsync(containerId, executionToken);

            await AwaitLogsAsync(logTask);

            stopwatch.Stop();

            return CreateResult(
                step,
                exitCode,
                ResolveStatus(exitCode, step.Failure),
                stopwatch.Elapsed);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            return CreateResult(step, -1, StepStatus.Canceled, stopwatch.Elapsed);
        }
        catch (OperationCanceledException) when (timeoutCts?.IsCancellationRequested == true)
        {
            stopwatch.Stop();
            return CreateResult(step, -1, StepStatus.Failed, stopwatch.Elapsed);
        }
        catch
        {
            stopwatch.Stop();
            return CreateResult(step, -1, StepStatus.Failed, stopwatch.Elapsed);
        }
        finally
        {
            if (containerId is not null)
            {
                await RemoveContainerAsync(containerId);
            }
        }
    }

    private DockerContainerSpec CreateContainerSpec(StepExecutionContext context)
    {
        var step = context.Step;

        var workingDirectory = string.IsNullOrWhiteSpace(step.WorkingDirectory)
            ? "/workspace"
            : step.WorkingDirectory;

        var environment = new Dictionary<string, string>(context.Environment);

        foreach (var item in step.Environment)
        {
            environment[item.Key] = item.Value;
        }

        var volumes = new List<DockerVolumeMount>();

        var workspacePath = PrepareWorkspacePath(context.WorkspacePath);

        if (workspacePath is not null)
        {
            volumes.Add(new DockerVolumeMount
            {
                Source = workspacePath,
                Target = workingDirectory,
                ReadOnly = false,
                Type = DockerVolumeMountType.Bind
            });
        }

        volumes.AddRange(step.Volumes
            .Where(volume =>
                !string.IsNullOrWhiteSpace(volume.Host) &&
                !string.IsNullOrWhiteSpace(volume.Container))
            .Select(volume => new DockerVolumeMount
            {
                Source = volume.Host,
                Target = volume.Container,
                ReadOnly = volume.ReadOnly,
                Type = DockerVolumeMountType.Bind
            }));

        var (entrypoint, command) = CreateCommand(step);

        return new DockerContainerSpec
        {
            Name = CreateContainerName(context.TaskId, step.Name),
            Image = step.Image,
            WorkingDirectory = workingDirectory,
            NetworkMode = step.NetworkMode,
            User = step.User,
            Privileged = step.Privileged,
            Entrypoint = entrypoint,
            Command = command,
            Volumes = volumes,
            Environment = environment,
            Labels = new Dictionary<string, string>
            {
                ["pipelines.managed"] = "true",
                ["pipelines.task"] = context.TaskId.ToString(),
                ["pipelines.step"] = step.Name
            }
        };
    }

    private static (IReadOnlyList<string> Entrypoint, IReadOnlyList<string> Command) CreateCommand(
        StepConfiguration step)
    {
        if (step.Commands.Count > 0)
        {
            var script = string.Join('\n', step.Commands);
            return (["/bin/sh", "-c"], [$"set -e\n{script}"]);
        }

        return (
            step.Entrypoint ?? [],
            step.Command ?? []);
    }

    private async Task StreamLogsAsync(
        string containerId,
        string stepName,
        CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var log in _dockerManager.StreamLogsAsync(containerId, cancellationToken))
            {
                var text = log.Text.TrimEnd();

                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                Console.WriteLine($"[{stepName}] [{log.Stream}] {text}");
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static async Task AwaitLogsAsync(Task logTask)
    {
        try
        {
            await logTask;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task RemoveContainerAsync(string containerId)
    {
        try
        {
            await _dockerManager.RemoveContainerAsync(
                containerId,
                force: true,
                cancellationToken: CancellationToken.None);
        }
        catch
        {
            // Cleanup should not hide the original step result.
        }
    }

    private static CancellationTokenSource? CreateTimeoutTokenSource(
        StepConfiguration step,
        CancellationToken cancellationToken)
    {
        if (step.TimeoutMinutes is not > 0)
        {
            return null;
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromMinutes(step.TimeoutMinutes.Value));
        return cts;
    }

    private static StepStatus ResolveStatus(int exitCode, FailurePolicy failurePolicy)
    {
        if (exitCode == 0)
        {
            return StepStatus.Succeeded;
        }

        return failurePolicy == FailurePolicy.Ignore
            ? StepStatus.Succeeded
            : StepStatus.Failed;
    }

    private static StepResult CreateResult(
        StepConfiguration step,
        int exitCode,
        StepStatus status,
        TimeSpan duration)
    {
        return new StepResult
        {
            StepName = step.Name,
            ExitCode = exitCode,
            Status = status,
            Duration = duration
        };
    }

    private static string? PrepareWorkspacePath(string workspacePath)
    {
        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(workspacePath);
        Directory.CreateDirectory(fullPath);

        return fullPath;
    }


    private static string CreateContainerName(Guid taskId, string stepName)
    {
        var stepSegment = string.Concat(stepName
            .Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '-'))
            .Trim('-');

        return $"pipelines-{taskId:N}-{(stepSegment.Length > 0 ? stepSegment : "step")}";
    }
}