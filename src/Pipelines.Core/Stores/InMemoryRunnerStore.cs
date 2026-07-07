using System.Collections.Concurrent;
using Pipelines.Core.Entities;

namespace Pipelines.Core.Stores;

public sealed class InMemoryRunnerStore : IRunnerStore
{
    private readonly ConcurrentDictionary<Guid, RunnerRecord> _runners = new();

    public Task UpsertAsync(
        RunnerRecord runner,
        CancellationToken cancellationToken = default)
    {
        _runners[runner.RunnerId] = runner;
        return Task.CompletedTask;
    }

    public Task<RunnerRecord?> GetAsync(
        Guid runnerId,
        CancellationToken cancellationToken = default)
    {
        _runners.TryGetValue(runnerId, out var runner);
        return Task.FromResult(runner);
    }

    public Task<IReadOnlyList<RunnerRecord>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var runners = _runners.Values
            .OrderBy(runner => runner.RegisteredAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<RunnerRecord>>(runners);
    }
}
