using System.Diagnostics;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationProcessRunner
{
    public async Task<WorkstationProcessResult> RunAsync(
        WorkstationProcessSpec spec,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var timeoutCts = CreateTimeoutTokenSource(spec, cancellationToken);
        var executionToken = timeoutCts?.Token ?? cancellationToken;

        try
        {
            using var process = new Process
            {
                StartInfo = CreateStartInfo(spec),
                EnableRaisingEvents = true
            };

            var outputCompleted = CreateOutputCompletionSource();
            var errorCompleted = CreateOutputCompletionSource();

            process.OutputDataReceived += (_, args) =>
            {
                if (args.Data is null)
                {
                    outputCompleted.TrySetResult();
                    return;
                }

                WriteLog(spec.StepName, "stdout", args.Data);
            };

            process.ErrorDataReceived += (_, args) =>
            {
                if (args.Data is null)
                {
                    errorCompleted.TrySetResult();
                    return;
                }

                WriteLog(spec.StepName, "stderr", args.Data);
            };

            if (!process.Start())
            {
                stopwatch.Stop();
                return CreateResult(-1, StepStatus.Failed, stopwatch.Elapsed);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await WaitForExitAsync(process, executionToken);
            await Task.WhenAll(outputCompleted.Task, errorCompleted.Task);

            stopwatch.Stop();
            return CreateResult(
                process.ExitCode,
                process.ExitCode == 0 ? StepStatus.Succeeded : StepStatus.Failed,
                stopwatch.Elapsed);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            return CreateResult(-1, StepStatus.Canceled, stopwatch.Elapsed);
        }
        catch (OperationCanceledException) when (timeoutCts?.IsCancellationRequested == true)
        {
            stopwatch.Stop();
            return CreateResult(-1, StepStatus.Failed, stopwatch.Elapsed);
        }
        catch
        {
            stopwatch.Stop();
            return CreateResult(-1, StepStatus.Failed, stopwatch.Elapsed);
        }
    }

    private static ProcessStartInfo CreateStartInfo(WorkstationProcessSpec spec)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = spec.FileName,
            WorkingDirectory = spec.WorkingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.Environment.Clear();

        foreach (var argument in spec.Arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        foreach (var item in spec.Environment)
        {
            startInfo.Environment[item.Key] = item.Value;
        }

        return startInfo;
    }

    private static async Task WaitForExitAsync(
        Process process,
        CancellationToken cancellationToken)
    {
        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            KillProcess(process);
            throw;
        }
    }

    private static void KillProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // Process cleanup should not hide the cancellation result.
        }
    }

    private static CancellationTokenSource? CreateTimeoutTokenSource(
        WorkstationProcessSpec spec,
        CancellationToken cancellationToken)
    {
        if (spec.TimeoutMinutes is not > 0)
        {
            return null;
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromMinutes(spec.TimeoutMinutes.Value));
        return cts;
    }

    private static TaskCompletionSource CreateOutputCompletionSource()
    {
        return new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private static WorkstationProcessResult CreateResult(
        int exitCode,
        StepStatus status,
        TimeSpan duration)
    {
        return new WorkstationProcessResult
        {
            ExitCode = exitCode,
            Status = status,
            Duration = duration
        };
    }

    private static void WriteLog(string stepName, string stream, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            Console.WriteLine($"[{stepName}] [{stream}] {text}");
        }
    }
}
