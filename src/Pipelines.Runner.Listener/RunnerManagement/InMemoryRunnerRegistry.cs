using Pipelines.Core.Scheduling;
using System.Collections.Concurrent;

namespace Pipelines.Runner.Listener.RunnerManagement;

/// <summary>
/// In-memory implementation of runner registry
/// </summary>
public class InMemoryRunnerRegistry : IRunnerRegistry
{
    private readonly ConcurrentDictionary<string, RunnerInfo> _runners = new();
    private readonly ConcurrentDictionary<string, int> _runnerJobCounts = new();

    public Task RegisterRunnerAsync(RunnerInfo runner, CancellationToken cancellationToken = default)
    {
        _runners[runner.Id] = runner;
        _runnerJobCounts[runner.Id] = 0;
        return Task.CompletedTask;
    }

    public Task HeartbeatAsync(string runnerId, CancellationToken cancellationToken = default)
    {
        if (_runners.TryGetValue(runnerId, out var runner))
        {
            var updatedRunner = runner with { LastHeartbeat = DateTimeOffset.UtcNow };
            _runners[runnerId] = updatedRunner;
        }
        return Task.CompletedTask;
    }

    public Task UnregisterRunnerAsync(string runnerId, CancellationToken cancellationToken = default)
    {
        _runners.TryRemove(runnerId, out _);
        _runnerJobCounts.TryRemove(runnerId, out _);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<RunnerInfo>> GetActiveRunnersAsync(CancellationToken cancellationToken = default)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-2); // Consider runners inactive after 2 minutes
        var activeRunners = _runners.Values
            .Where(r => r.LastHeartbeat > cutoff && r.Status != RunnerStatus.Offline)
            .ToList();
        
        return Task.FromResult<IEnumerable<RunnerInfo>>(activeRunners);
    }

    public Task<IEnumerable<RunnerInfo>> GetAvailableRunnersAsync(string[] requiredCapabilities, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-2);
        var availableRunners = _runners.Values
            .Where(r => r.LastHeartbeat > cutoff && 
                       (r.Status == RunnerStatus.Idle || r.Status == RunnerStatus.Busy) &&
                       r.Status != RunnerStatus.Draining &&
                       HasRequiredCapabilities(r, requiredCapabilities) &&
                       CanTakeMoreJobs(r))
            .ToList();
        
        return Task.FromResult<IEnumerable<RunnerInfo>>(availableRunners);
    }

    public Task SetRunnerStatusAsync(string runnerId, RunnerStatus status, CancellationToken cancellationToken = default)
    {
        if (_runners.TryGetValue(runnerId, out var runner))
        {
            var updatedRunner = runner with { Status = status };
            _runners[runnerId] = updatedRunner;
            
            // Reset job count when going idle
            if (status == RunnerStatus.Idle)
            {
                _runnerJobCounts[runnerId] = 0;
            }
        }
        return Task.CompletedTask;
    }

    public Task CleanupStaleRunnersAsync(TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTimeOffset.UtcNow - maxAge;
        var staleRunners = _runners.Values
            .Where(r => r.LastHeartbeat < cutoff)
            .Select(r => r.Id)
            .ToList();

        foreach (var runnerId in staleRunners)
        {
            _runners.TryRemove(runnerId, out _);
            _runnerJobCounts.TryRemove(runnerId, out _);
        }

        return Task.CompletedTask;
    }

    public void IncrementJobCount(string runnerId)
    {
        _runnerJobCounts.AddOrUpdate(runnerId, 1, (_, count) => count + 1);
    }

    public void DecrementJobCount(string runnerId)
    {
        _runnerJobCounts.AddOrUpdate(runnerId, 0, (_, count) => Math.Max(0, count - 1));
    }

    private static bool HasRequiredCapabilities(RunnerInfo runner, string[] requiredCapabilities)
    {
        if (requiredCapabilities.Length == 0) return true;
        
        return requiredCapabilities.All(req => 
            runner.Capabilities.Contains(req, StringComparer.OrdinalIgnoreCase));
    }

    private bool CanTakeMoreJobs(RunnerInfo runner)
    {
        _runnerJobCounts.TryGetValue(runner.Id, out var currentJobs);
        return currentJobs < runner.MaxConcurrentJobs;
    }
}
