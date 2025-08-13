using Pipelines.Core.Scheduling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Pipelines.Runner.Docker.Services;

/// <summary>
/// Service responsible for runner registration and heartbeat
/// </summary>
public class RunnerRegistrationService : BackgroundService
{
    private readonly HttpClient _http;
    private readonly ILogger<RunnerRegistrationService> _logger;
    private readonly RunnerConfiguration _config;
    private string? _runnerId;
    private readonly PeriodicTimer _heartbeatTimer;

    public RunnerRegistrationService(
        IHttpClientFactory factory,
        ILogger<RunnerRegistrationService> logger,
        RunnerConfiguration config)
    {
        _http = factory.CreateClient("scheduler-server");
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
            // Unregister on shutdown
            if (_runnerId != null)
            {
                await UnregisterAsync();
            }
        }
    }

    private async Task RegisterAsync(CancellationToken cancellationToken)
    {
        try
        {
            var request = new
            {
                Name = _config.Name,
                Capabilities = _config.Capabilities,
                MaxConcurrentJobs = _config.MaxConcurrentJobs,
                Version = _config.Version,
                Platform = Environment.OSVersion.Platform.ToString(),
                Labels = _config.Labels
            };

            var response = await _http.PostAsJsonAsync("/api/runners/register", request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                _runnerId = result.GetProperty("RunnerId").GetString();
                _logger.LogInformation("Successfully registered runner: {RunnerId}", _runnerId);
            }
            else
            {
                _logger.LogError("Failed to register runner: {StatusCode} - {Content}", 
                    response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
            }
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
            var response = await _http.PostAsync($"/api/runners/{_runnerId}/heartbeat", null, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Heartbeat failed: {StatusCode}", response.StatusCode);
                
                // If runner not found, re-register
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Runner not found on server, re-registering...");
                    _runnerId = null;
                    await RegisterAsync(cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error sending heartbeat");
        }
    }

    private async Task UnregisterAsync()
    {
        if (_runnerId == null) return;

        try
        {
            await _http.DeleteAsync($"/api/runners/{_runnerId}");
            _logger.LogInformation("Runner unregistered: {RunnerId}", _runnerId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error unregistering runner");
        }
    }

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
