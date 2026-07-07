using Pipelines.Abstractions;
using Pipelines.Core.Entities;
using Pipelines.Core.Models;
using Pipelines.Core.Stores;

namespace Pipelines;

public sealed class RunnerRegistry : IRunnerRegistry
{
    private readonly IRunnerStore _store;

    public RunnerRegistry(IRunnerStore store)
    {
        _store = store;
    }

    public async Task<RunnerRecord> RegisterAsync(
        RunnerProfile profile,
        TimeSpan heartbeatTimeout,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var existing = await _store.GetAsync(profile.RunnerId, cancellationToken);

        var runner = existing ?? new RunnerRecord
        {
            RunnerId = profile.RunnerId,
            RegisteredAt = now
        };

        runner.Profile = profile;
        runner.Status = RunnerStatus.Online;
        runner.LastSeenAt = now;
        runner.ExpiresAt = now.Add(heartbeatTimeout);

        await _store.UpsertAsync(runner, cancellationToken);
        return runner;
    }

    public Task<RunnerRecord> HeartbeatAsync(
        RunnerProfile profile,
        TimeSpan heartbeatTimeout,
        CancellationToken cancellationToken = default)
    {
        return RegisterAsync(profile, heartbeatTimeout, cancellationToken);
    }

    public async Task<IReadOnlyList<RunnerRecord>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var runners = await _store.ListAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;

        foreach (var runner in runners)
        {
            var status = runner.ExpiresAt > now
                ? RunnerStatus.Online
                : RunnerStatus.Offline;

            if (runner.Status == status)
            {
                continue;
            }

            runner.Status = status;
            await _store.UpsertAsync(runner, cancellationToken);
        }

        return runners;
    }
}
