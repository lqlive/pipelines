using System.Collections.Concurrent;
using Pipelines.Core.Entities;
using Pipelines.Core.Models;

namespace Pipelines.Core.Stores;

public sealed class InMemoryTaskStore : ITaskStore
{
    private readonly ConcurrentDictionary<Guid, TaskRecord> _tasks = new();
    public Task EnqueueAsync(
        TaskRecord task,
        CancellationToken cancellationToken = default)
    {
        _tasks[task.TaskId] = task;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TaskRecord>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var tasks = _tasks.Values
            .OrderBy(task => task.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<TaskRecord>>(tasks);
    }

    public Task<IReadOnlyList<TaskRecord>> GetPendingAsync(
        CancellationToken cancellationToken = default)
    {
        var tasks = _tasks.Values
            .Where(task => task.State == TaskState.Pending)
            .OrderBy(task => task.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<TaskRecord>>(tasks);
    }

    public Task<IReadOnlyList<TaskRecord>> GetLeasedAsync(
        CancellationToken cancellationToken = default)
    {
        var tasks = _tasks.Values
            .Where(task => task.State == TaskState.Leased)
            .OrderBy(task => task.LeaseExpiresAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<TaskRecord>>(tasks);
    }

    public Task<TaskRecord?> GetAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        _tasks.TryGetValue(taskId, out var task);
        return Task.FromResult(task);
    }

    public Task UpdateAsync(
        TaskRecord task,
        CancellationToken cancellationToken = default)
    {
        _tasks[task.TaskId] = task;
        return Task.CompletedTask;
    }

    public Task CompleteAsync(
        Guid taskId,
        PipelineResult result,
        CancellationToken cancellationToken = default)
    {
        if (_tasks.TryGetValue(taskId, out var task))
        {
            task.State = TaskState.Completed;
            task.Result = result;
            task.RunnerId = null;
            task.LeaseToken = string.Empty;
            task.LeaseExpiresAt = null;
            _tasks[taskId] = task;
        }

        return Task.CompletedTask;
    }
}