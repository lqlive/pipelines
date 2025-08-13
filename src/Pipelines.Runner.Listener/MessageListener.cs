using Pipelines.Core.Scheduling;
using Pipelines.Runner.Listener.Configuration;
using Pipelines.Runner.Listener.JobDispatcher;
using Pipelines.Runner.Listener.RunnerManagement;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pipelines.Runner.Listener;

/// <summary>
/// Main message listener service that coordinates job dispatching and runner management
/// Similar to GitHub Actions Runner.Listener architecture
/// </summary>
public class MessageListener : BackgroundService
{
    private readonly IJobScheduler _jobScheduler;
    private readonly IRunnerRegistry _runnerRegistry;
    private readonly ListenerConfiguration _config;
    private readonly ILogger<MessageListener> _logger;

    public MessageListener(
        IJobScheduler jobScheduler,
        IRunnerRegistry runnerRegistry,
        IOptions<ListenerConfiguration> config,
        ILogger<MessageListener> logger)
    {
        _jobScheduler = jobScheduler;
        _runnerRegistry = runnerRegistry;
        _config = config.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🎧 Message Listener started");
        _logger.LogInformation("📊 Configuration: MaxQueueSize={MaxQueueSize}, RunnerStaleTimeout={RunnerStaleTimeout}",
            _config.JobQueue.MaxQueueSize, _config.RunnerManagement.RunnerStaleTimeout);

        var tasks = new List<Task>
        {
            MonitorQueueStatistics(stoppingToken),
            MonitorRunnerHealth(stoppingToken)
        };

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("🛑 Message Listener stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Message Listener failed");
            throw;
        }
    }

    private async Task MonitorQueueStatistics(CancellationToken cancellationToken)
    {
        if (!_config.Performance.EnableMetrics) return;

        using var timer = new PeriodicTimer(_config.Performance.MetricsInterval);

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                var stats = await _jobScheduler.GetStatisticsAsync(cancellationToken);
                var activeRunners = await _runnerRegistry.GetActiveRunnersAsync(cancellationToken);
                
                _logger.LogInformation(
                    "📈 Queue Stats: Pending={Pending}, Running={Running}, ActiveRunners={ActiveRunners}, IdleRunners={IdleRunners}",
                    stats.PendingJobs, stats.RunningJobs, stats.ActiveRunners, stats.IdleRunners);
                
                if (stats.PendingJobs > _config.JobQueue.MaxQueueSize * 0.8)
                {
                    _logger.LogWarning("⚠️ Queue is {Percentage:P0} full ({Pending}/{Max})", 
                        (double)stats.PendingJobs / _config.JobQueue.MaxQueueSize, 
                        stats.PendingJobs, _config.JobQueue.MaxQueueSize);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to collect queue statistics");
            }
        }
    }

    private async Task MonitorRunnerHealth(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                var activeRunners = await _runnerRegistry.GetActiveRunnersAsync(cancellationToken);
                var runnersByStatus = activeRunners.GroupBy(r => r.Status).ToDictionary(g => g.Key, g => g.Count());

                _logger.LogDebug("🏃 Runner Health: {RunnerStats}",
                    string.Join(", ", runnersByStatus.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                // Check for overloaded runners
                var overloadedRunners = activeRunners.Where(r => r.Status == RunnerStatus.Busy)
                    .Where(r => r.MaxConcurrentJobs > _config.RunnerManagement.MaxConcurrentJobsPerRunner)
                    .ToList();

                if (overloadedRunners.Any())
                {
                    _logger.LogWarning("⚠️ {Count} runners may be overloaded", overloadedRunners.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to monitor runner health");
            }
        }
    }
}
