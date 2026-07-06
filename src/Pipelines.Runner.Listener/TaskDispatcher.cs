using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Listener;

public sealed class TaskDispatcher : ITaskDispatcher
{
    private readonly ITaskQueue _taskQueue;
    private readonly IPipelineRunner _pipelineRunner;
    private readonly RunnerProfile _runnerProfile;
    private readonly ILogger<TaskDispatcher> _logger;
    private readonly TaskDispatcherOptions _options;
    private readonly CancellationTokenSource _stopCts = new();

    public TaskDispatcher(
        ITaskQueue taskQueue,
        IPipelineRunner pipelineRunner,
        RunnerProfile runnerProfile,
        ILogger<TaskDispatcher> logger,
        IOptions<TaskDispatcherOptions> options)
    {
        _taskQueue = taskQueue;
        _pipelineRunner = pipelineRunner;
        _runnerProfile = runnerProfile;
        _logger = logger;
        _options = options.Value;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _stopCts.Token);

        var token = linkedCts.Token;

        while (!token.IsCancellationRequested)
        {
            var task = await _taskQueue.TryAcquireAsync(_runnerProfile, token);

            if (task is null)
            {
                await Task.Delay(_options.IdleDelay, token);
                continue;
            }

            await ExecuteTaskAsync(task, token);
        }
    }

    public Task CancelAsync(CancellationToken cancellationToken = default)
    {
        _stopCts.Cancel();
        return Task.CompletedTask;
    }

    private async Task ExecuteTaskAsync(
        AcquiredTask task,
        CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;

        using var executionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var leaseTask = RenewLeaseAsync(task, executionCts);

        PipelineResult result;

        try
        {
            var context = new PipelineExecutionContext
            {
                TaskId = task.TaskId,
                RunnerId = task.RunnerId,
                Pipeline = task.Pipeline,
                WorkspacePath = task.WorkspacePath,
                Variables = task.Variables
            };

            result = await _pipelineRunner.RunAsync(context, executionCts.Token);
        }
        catch (OperationCanceledException)
        {
            result = new PipelineResult
            {
                TaskId = task.TaskId,
                Status = PipelineStatus.Canceled,
                StartedAt = startedAt,
                FinishedAt = DateTimeOffset.UtcNow,
                ErrorMessage = "Pipeline execution was canceled."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pipeline task {TaskId} failed.", task.TaskId);

            result = new PipelineResult
            {
                TaskId = task.TaskId,
                Status = PipelineStatus.Failed,
                StartedAt = startedAt,
                FinishedAt = DateTimeOffset.UtcNow,
                ErrorMessage = ex.Message
            };
        }
        finally
        {
            await executionCts.CancelAsync();

            try
            {
                await leaseTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        await _taskQueue.CompleteAsync(
            task.TaskId,
            task.RunnerId,
            task.LeaseToken,
            result,
            CancellationToken.None);
    }

    private async Task RenewLeaseAsync(
        AcquiredTask task,
        CancellationTokenSource executionCts)
    {
        while (!executionCts.IsCancellationRequested)
        {
            await Task.Delay(_options.LeaseRenewalInterval, executionCts.Token);

            var renewed = await _taskQueue.RenewLeaseAsync(
                task.TaskId,
                task.RunnerId,
                task.LeaseToken,
                _options.LeaseDuration,
                executionCts.Token);

            if (!renewed)
            {
                _logger.LogWarning("Lease renewal failed for task {TaskId}.", task.TaskId);
                await executionCts.CancelAsync();
                return;
            }
        }
    }
}