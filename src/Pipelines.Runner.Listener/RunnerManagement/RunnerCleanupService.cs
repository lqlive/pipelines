using Pipelines.Core.Scheduling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pipelines.Runner.Listener.RunnerManagement;

/// <summary>
/// Background service to clean up stale runners
/// </summary>
public class RunnerCleanupService : BackgroundService
{
    private readonly IRunnerRegistry _registry;
    private readonly ILogger<RunnerCleanupService> _logger;
    private readonly PeriodicTimer _timer;

    public RunnerCleanupService(IRunnerRegistry registry, ILogger<RunnerCleanupService> logger)
    {
        _registry = registry;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromMinutes(1)); // Clean up every minute
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Runner cleanup service started");

        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var maxAge = TimeSpan.FromMinutes(5); // Consider runners stale after 5 minutes without heartbeat
                await _registry.CleanupStaleRunnersAsync(maxAge, stoppingToken);
                
                // Log runner statistics
                var activeRunners = await _registry.GetActiveRunnersAsync(stoppingToken);
                var runnerCount = activeRunners.Count();
                
                if (runnerCount > 0)
                {
                    _logger.LogDebug("Active runners: {RunnerCount}", runnerCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during runner cleanup");
            }
        }

        _logger.LogInformation("Runner cleanup service stopped");
    }

    public override void Dispose()
    {
        _timer.Dispose();
        base.Dispose();
    }
}
