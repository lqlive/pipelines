using Pipelines.Core.Scheduling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Grpc.Net.Client;
using Pipelines.Proto;

namespace Pipelines.Runner.Docker.Services;

/// <summary>
/// Service responsible for runner registration and heartbeat
/// </summary>
public class RunnerRegistrationService : BackgroundService
{
    private readonly RunnerService.RunnerServiceClient _client;
    private readonly ILogger<RunnerRegistrationService> _logger;
    private readonly RunnerConfiguration _config;
    private string? _runnerId;
    private readonly PeriodicTimer _heartbeatTimer;

    public RunnerRegistrationService(
        ILogger<RunnerRegistrationService> logger,
        RunnerConfiguration config)
    {
        var baseUrl = Environment.GetEnvironmentVariable("SCHEDULER_SERVER") ?? "http://localhost:5170";
        var channel = GrpcChannel.ForAddress(baseUrl);
        _client = new RunnerService.RunnerServiceClient(channel);
        _logger = logger;
        _config = config;
        _heartbeatTimer = new PeriodicTimer(TimeSpan.FromSeconds(30)); // Heartbeat every 30 seconds
    }

    public string? RunnerId => _runnerId;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Register runner first
            await RegisterAsync(stoppingToken);

            if (_runnerId == null)
            {
                _logger.LogError("Failed to register runner, stopping service");
                return;
            }

            _logger.LogInformation("Runner registered with ID: {RunnerId}", _runnerId);

            // Start heartbeat loop
            while (!stoppingToken.IsCancellationRequested && await _heartbeatTimer.WaitForNextTickAsync(stoppingToken))
            {
                await SendHeartbeatAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Runner registration service stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in runner registration service");
        }
        finally
        {
            // Unregister on shutdown (not implemented in gRPC yet)
        }
    }

    private async Task RegisterAsync(CancellationToken cancellationToken)
    {
        try
        {
            var resp = await _client.RegisterRunnerAsync(new RegisterRunnerRequest
            {
                Name = _config.Name,
                Capabilities = { _config.Capabilities },
                MaxConcurrentJobs = _config.MaxConcurrentJobs,
                Version = _config.Version,
                Platform = Environment.OSVersion.Platform.ToString(),
                Labels = { _config.Labels }
            }, cancellationToken: cancellationToken);
            _runnerId = resp.RunnerId;
            _logger.LogInformation("Successfully registered runner: {RunnerId}", _runnerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering runner");
        }
    }

    private async Task SendHeartbeatAsync(CancellationToken cancellationToken)
    {
        if (_runnerId == null) return;

        try
        {
            await _client.HeartbeatAsync(new HeartbeatRequest { RunnerId = _runnerId }, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error sending heartbeat");
        }
    }

    // private async Task UnregisterAsync() { }

    public override void Dispose()
    {
        _heartbeatTimer.Dispose();
        base.Dispose();
    }
}

/// <summary>
/// Runner configuration
/// </summary>
public class RunnerConfiguration
{
    public string Name { get; set; } = Environment.MachineName;
    public string[] Capabilities { get; set; } = ["docker", "linux", "ubuntu"];
    public int MaxConcurrentJobs { get; set; } = 1;
    public string Version { get; set; } = "1.0.0";
    public Dictionary<string, string> Labels { get; set; } = new();
}
