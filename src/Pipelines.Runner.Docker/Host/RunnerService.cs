using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Runner;
using Pipelines.Runner.Docker.Worker;
using Pipelines.Runner.Docker.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pipelines.Runner.Docker.Host;

public class RunnerService : BackgroundService
{
    private readonly IJobServerQueue _queue;
    private readonly JobRunner _jobRunner;
    private readonly RunnerRegistrationService _registrationService;
    private readonly ILogger<RunnerService> _logger;

    public RunnerService(
        IJobServerQueue queue, 
        JobRunner jobRunner, 
        RunnerRegistrationService registrationService,
        ILogger<RunnerService> logger)
    {
        _queue = queue;
        _jobRunner = jobRunner;
        _registrationService = registrationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Runner service started");
        
        // Wait for registration to complete
        while (_registrationService.RunnerId == null && !stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Waiting for runner registration...");
            await Task.Delay(1000, stoppingToken);
        }

        if (_registrationService.RunnerId == null)
        {
            _logger.LogError("Failed to register runner, stopping");
            return;
        }

        _logger.LogInformation("Runner registered with ID: {RunnerId}", _registrationService.RunnerId);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var build = await _queue.DequeueAsync<Build>(stoppingToken);
                if (build is null)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                _logger.LogInformation("Received build {BuildId} for repository {Repository}", 
                    build.Id, build.RepositoryUrl);

                await _jobRunner.RunAsync(build, stoppingToken);
                
                _logger.LogInformation("Completed build {BuildId} with status {Status}", 
                    build.Id, build.Status);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Runner service stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing build");
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}


