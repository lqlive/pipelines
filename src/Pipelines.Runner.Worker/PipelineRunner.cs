using Pipelines.Core.Abstractions;
using Pipelines.Core.Configuration;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Worker;

public sealed class PipelineRunner : IPipelineRunner
{
    private readonly IStepRunner _stepRunner;

    public PipelineRunner(IStepRunner stepRunner)
    {
        _stepRunner = stepRunner;
    }

    public async Task<PipelineResult> RunAsync(
        PipelineExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var results = new List<StepResult>();

        try
        {
            foreach (var step in context.Pipeline.Steps)
            {
                if (!HasCompletedDependencies(step, results))
                {
                    results.Add(Skipped(step));
                    continue;
                }

                var stepContext = new StepExecutionContext
                {
                    TaskId = context.TaskId,
                    WorkspacePath = context.WorkspacePath,
                    Step = step,
                    Environment = CreateEnvironment(context)
                };

                var result = await _stepRunner.RunAsync(stepContext, cancellationToken);
                results.Add(result);

                if (result.Status == StepStatus.Canceled)
                {
                    return Complete(
                        context.TaskId,
                        PipelineStatus.Canceled,
                        results,
                        startedAt,
                        $"Step '{step.Name}' was canceled.");
                }

                if (result.Status == StepStatus.Failed)
                {
                    return Complete(
                        context.TaskId,
                        PipelineStatus.Failed,
                        results,
                        startedAt,
                        $"Step '{step.Name}' failed.");
                }
            }

            return Complete(
                context.TaskId,
                PipelineStatus.Succeeded,
                results,
                startedAt);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Complete(
                context.TaskId,
                PipelineStatus.Canceled,
                results,
                startedAt,
                "Pipeline execution was canceled.");
        }
        catch (Exception ex)
        {
            return Complete(
                context.TaskId,
                PipelineStatus.Failed,
                results,
                startedAt,
                ex.Message);
        }
    }

    private static bool HasCompletedDependencies(
        StepConfiguration step,
        IReadOnlyCollection<StepResult> results)
    {
        if (step.DependsOn.Count == 0)
        {
            return true;
        }

        return step.DependsOn.All(dependency =>
            results.Any(result =>
                string.Equals(result.StepName, dependency, StringComparison.OrdinalIgnoreCase) &&
                result.Status == StepStatus.Succeeded));
    }

    private static IReadOnlyDictionary<string, string> CreateEnvironment(
        PipelineExecutionContext context)
    {
        return new Dictionary<string, string>(context.Variables)
        {
            ["PIPELINES_TASK_ID"] = context.TaskId.ToString(),
            ["PIPELINES_RUNNER_ID"] = context.RunnerId.ToString(),
            ["PIPELINES_PIPELINE_NAME"] = context.Pipeline.Name
        };
    }

    private static StepResult Skipped(StepConfiguration step)
    {
        return new StepResult
        {
            StepName = step.Name,
            ExitCode = 0,
            Status = StepStatus.Skipped,
            Duration = TimeSpan.Zero
        };
    }

    private static PipelineResult Complete(
        Guid taskId,
        PipelineStatus status,
        IReadOnlyList<StepResult> steps,
        DateTimeOffset startedAt,
        string? errorMessage = null)
    {
        return new PipelineResult
        {
            TaskId = taskId,
            Status = status,
            Steps = steps,
            StartedAt = startedAt,
            FinishedAt = DateTimeOffset.UtcNow,
            ErrorMessage = errorMessage
        };
    }
}