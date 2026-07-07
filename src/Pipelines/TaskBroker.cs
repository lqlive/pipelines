using Pipelines.Abstractions;
using Pipelines.Core.Configuration;
using Pipelines.Core.Entities;
using Pipelines.Core.Models;
using Pipelines.Core.Stores;

namespace Pipelines;

public sealed class TaskBroker : ITaskBroker
{
    private readonly ITaskStore _store;
    private readonly IRunnerSelector _runnerSelector;

    public TaskBroker(
        ITaskStore store,
        IRunnerSelector runnerSelector)
    {
        _store = store;
        _runnerSelector = runnerSelector;
    }
    public async Task<Guid> EnqueueAsync(
        PipelineConfiguration pipeline,
        string workspacePath,
        Dictionary<string, string> variables,
        CancellationToken cancellationToken = default)
    {
        var task = new TaskRecord
        {
            Pipeline = pipeline,
            WorkspacePath = workspacePath,
            Variables = variables
        };
        await _store.EnqueueAsync(task, cancellationToken);
        return task.TaskId;
    }
    public async Task<AcquiredTask?> TryAcquireAsync(
        RunnerProfile profile,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        await RequeueExpiredLeasesAsync(cancellationToken);

        if (await IsAtCapacityAsync(profile, cancellationToken))
        {
            return null;
        }

        var pendingTasks = await _store.GetPendingAsync(cancellationToken);
        var task = pendingTasks.FirstOrDefault(task =>
            _runnerSelector.Matches(task.Pipeline, profile));
        if (task is null)
        {
            return null;
        }

        task.State = TaskState.Leased;
        task.RunnerId = profile.RunnerId;
        task.LeaseToken = Guid.NewGuid().ToString("N");
        task.LeaseExpiresAt = DateTimeOffset.UtcNow.Add(leaseDuration);
        task.Attempt++;

        await _store.UpdateAsync(task, cancellationToken);

        return new AcquiredTask
        {
            TaskId = task.TaskId,
            RunnerId = profile.RunnerId,
            LeaseToken = task.LeaseToken,
            LeaseExpiresAt = task.LeaseExpiresAt.Value,
            Attempt = task.Attempt,
            Pipeline = task.Pipeline,
            WorkspacePath = task.WorkspacePath,
            Variables = task.Variables
        };
    }

    public async Task<bool> RenewLeaseAsync(
        Guid taskId,
        Guid runnerId,
        string leaseToken,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        var task = await _store.GetAsync(taskId, cancellationToken);

        if (task is null || !MatchesActiveLease(task, runnerId, leaseToken))
        {
            return false;
        }

        task.LeaseExpiresAt = DateTimeOffset.UtcNow.Add(leaseDuration);
        await _store.UpdateAsync(task, cancellationToken);

        return true;
    }

    public async Task<bool> CompleteAsync(
        Guid taskId,
        Guid runnerId,
        string leaseToken,
        PipelineResult result,
        CancellationToken cancellationToken = default)
    {
        var task = await _store.GetAsync(taskId, cancellationToken);

        if (task is null || !MatchesActiveLease(task, runnerId, leaseToken))
        {
            return false;
        }

        await _store.CompleteAsync(taskId, result, cancellationToken);
        return true;
    }

    private async Task<bool> IsAtCapacityAsync(
        RunnerProfile profile,
        CancellationToken cancellationToken)
    {
        var capacity = Math.Max(1, profile.Capacity);
        var leasedTasks = await _store.GetLeasedAsync(cancellationToken);
        var activeTasks = leasedTasks.Count(task => task.RunnerId == profile.RunnerId);

        return activeTasks >= capacity;
    }

    private async Task RequeueExpiredLeasesAsync(
        CancellationToken cancellationToken)
    {
        var leasedTasks = await _store.GetLeasedAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;
        foreach (var task in leasedTasks)
        {
            if (task.LeaseExpiresAt is null || task.LeaseExpiresAt > now)
            {
                continue;
            }
            task.State = TaskState.Pending;
            task.RunnerId = null;
            task.LeaseToken = string.Empty;
            task.LeaseExpiresAt = null;
            await _store.UpdateAsync(task, cancellationToken);
        }
    }

    private static bool MatchesActiveLease(TaskRecord? task,Guid runnerId,string leaseToken)
    {
        return task is
        {
            State: TaskState.Leased,
            LeaseExpiresAt: not null
        }
        && task.RunnerId == runnerId
        && task.LeaseToken == leaseToken
        && task.LeaseExpiresAt > DateTimeOffset.UtcNow;
    }
}