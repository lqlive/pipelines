using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;
using System.Collections.Concurrent;

namespace Pipelines.Runner.Listener.JobDispatcher;

/// <summary>
/// In-memory implementation of job queue with priority support
/// </summary>
public class InMemoryJobQueue : IJobQueue
{
    private readonly ConcurrentDictionary<JobPriority, ConcurrentQueue<QueuedJob>> _queues = new();
    private readonly ConcurrentDictionary<Guid, RunningJob> _runningJobs = new();
    private readonly ConcurrentDictionary<Guid, JobStatus> _jobStatuses = new();
    private readonly object _lock = new();

    public InMemoryJobQueue()
    {
        // Initialize priority queues
        foreach (var priority in Enum.GetValues<JobPriority>())
        {
            _queues[priority] = new ConcurrentQueue<QueuedJob>();
        }
    }

    public Task EnqueueAsync(Build build, JobPriority priority = JobPriority.Normal, CancellationToken cancellationToken = default)
    {
        var queuedJob = new QueuedJob(build, priority, DateTimeOffset.UtcNow);
        var jobStatus = new JobStatus(
            build.Id,
            BuildStatus.Pending,
            null,
            DateTimeOffset.UtcNow,
            null,
            null,
            priority
        );

        _queues[priority].Enqueue(queuedJob);
        _jobStatuses[build.Id] = jobStatus;

        build.Status = BuildStatus.Pending;
        
        return Task.CompletedTask;
    }

    public Task<Build?> DequeueAsync(string runnerId, string[] capabilities, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Check queues in priority order (Critical -> High -> Normal -> Low)
            var priorities = Enum.GetValues<JobPriority>().OrderByDescending(p => p);
            
            foreach (var priority in priorities)
            {
                var queue = _queues[priority];
                
                while (queue.TryDequeue(out var queuedJob))
                {
                    var build = queuedJob.Build;
                    
                    // Check if runner can handle this job's requirements
                    if (CanRunnerHandleJob(build, capabilities))
                    {
                        // Mark as running
                        var runningJob = new RunningJob(build, runnerId, DateTimeOffset.UtcNow);
                        _runningJobs[build.Id] = runningJob;
                        
                        // Update job status
                        var updatedStatus = _jobStatuses[build.Id] with 
                        { 
                            Status = BuildStatus.Running,
                            RunnerId = runnerId,
                            StartedAt = DateTimeOffset.UtcNow
                        };
                        _jobStatuses[build.Id] = updatedStatus;
                        
                        build.Status = BuildStatus.Running;
                        build.StartedAt = DateTimeOffset.UtcNow;
                        
                        return Task.FromResult<Build?>(build);
                    }
                    else
                    {
                        // Re-queue if runner can't handle it
                        queue.Enqueue(queuedJob);
                        break; // Try next priority level
                    }
                }
            }
        }
        
        return Task.FromResult<Build?>(null);
    }

    public Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
    {
        var count = _queues.Values.Sum(q => q.Count);
        return Task.FromResult(count);
    }

    public Task<int> GetRunningCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_runningJobs.Count);
    }

    public Task CompleteJobAsync(Guid buildId, BuildStatus status, CancellationToken cancellationToken = default)
    {
        _runningJobs.TryRemove(buildId, out _);
        
        if (_jobStatuses.TryGetValue(buildId, out var jobStatus))
        {
            var updatedStatus = jobStatus with 
            { 
                Status = status,
                CompletedAt = DateTimeOffset.UtcNow
            };
            _jobStatuses[buildId] = updatedStatus;
        }
        
        return Task.CompletedTask;
    }

    public Task<bool> CancelJobAsync(Guid buildId, CancellationToken cancellationToken = default)
    {
        // Try to remove from running jobs first
        if (_runningJobs.TryGetValue(buildId, out var runningJob))
        {
            runningJob.Build.CancellationRequested = true;
            
            if (_jobStatuses.TryGetValue(buildId, out var jobStatus))
            {
                var updatedStatus = jobStatus with { Status = BuildStatus.Canceled };
                _jobStatuses[buildId] = updatedStatus;
            }
            
            return Task.FromResult(true);
        }

        // Try to remove from pending queues
        lock (_lock)
        {
            foreach (var queue in _queues.Values)
            {
                var items = new List<QueuedJob>();
                var found = false;
                
                while (queue.TryDequeue(out var item))
                {
                    if (item.Build.Id == buildId)
                    {
                        found = true;
                        item.Build.Status = BuildStatus.Canceled;
                        
                        if (_jobStatuses.TryGetValue(buildId, out var jobStatus))
                        {
                            var updatedStatus = jobStatus with 
                            { 
                                Status = BuildStatus.Canceled,
                                CompletedAt = DateTimeOffset.UtcNow
                            };
                            _jobStatuses[buildId] = updatedStatus;
                        }
                    }
                    else
                    {
                        items.Add(item);
                    }
                }
                
                // Re-enqueue non-cancelled items
                foreach (var item in items)
                {
                    queue.Enqueue(item);
                }
                
                if (found) return Task.FromResult(true);
            }
        }
        
        return Task.FromResult(false);
    }

    public Task<JobStatus?> GetJobStatusAsync(Guid buildId, CancellationToken cancellationToken = default)
    {
        _jobStatuses.TryGetValue(buildId, out var status);
        return Task.FromResult(status);
    }

    private static bool CanRunnerHandleJob(Build build, string[] runnerCapabilities)
    {
        // Basic capability matching - can be extended
        // For now, assume all runners can handle all jobs
        // Future: check required platforms, tools, etc.
        
        var requiredCapabilities = GetJobRequiredCapabilities(build);
        return requiredCapabilities.All(req => runnerCapabilities.Contains(req, StringComparer.OrdinalIgnoreCase));
    }

    private static string[] GetJobRequiredCapabilities(Build build)
    {
        var capabilities = new List<string> { "docker" }; // All jobs require Docker
        
        // Add platform-specific capabilities based on steps
        foreach (var step in build.Steps)
        {
            if (!string.IsNullOrEmpty(step.Image))
            {
                // Extract platform from image (e.g., "ubuntu", "windows", etc.)
                var imageParts = step.Image.Split(':');
                if (imageParts.Length > 0)
                {
                    capabilities.Add($"image-{imageParts[0]}");
                }
            }
        }
        
        return capabilities.Distinct().ToArray();
    }

    private record QueuedJob(Build Build, JobPriority Priority, DateTimeOffset QueuedAt);
    private record RunningJob(Build Build, string RunnerId, DateTimeOffset StartedAt);
}
