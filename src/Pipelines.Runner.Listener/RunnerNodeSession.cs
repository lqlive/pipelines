using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Listener;

public sealed class RunnerNodeSession : IHostedService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private static readonly TimeSpan RenewInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan SessionTtl = TimeSpan.FromMinutes(2);

    private readonly HttpClient _httpClient;
    private readonly RunnerProfile _profile;
    private readonly ILogger<RunnerNodeSession> _logger;
    private CancellationTokenSource? _sessionCts;
    private Task? _sessionTask;

    public RunnerNodeSession(
        HttpClient httpClient,
        RunnerProfile profile,
        ILogger<RunnerNodeSession> logger)
    {
        _httpClient = httpClient;
        _profile = profile;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _sessionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _sessionTask = RunAsync(_sessionCts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_sessionCts is null || _sessionTask is null)
        {
            return;
        }

        await _sessionCts.CancelAsync();

        try
        {
            await _sessionTask.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _sessionCts.Dispose();
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        await RegisterAsync(cancellationToken);

        using var timer = new PeriodicTimer(RenewInterval);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await RenewAsync(cancellationToken);
        }
    }

    private async Task RegisterAsync(CancellationToken cancellationToken)
    {
        try
        {
            await PostAsync("/runners/register", cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to register runner {RunnerId}.", _profile.RunnerId);
        }
    }

    private async Task RenewAsync(CancellationToken cancellationToken)
    {
        try
        {
            await PostAsync($"/runners/{_profile.RunnerId}/heartbeat", cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to renew runner session {RunnerId}.", _profile.RunnerId);
        }
    }

    private async Task PostAsync(
        string requestUri,
        CancellationToken cancellationToken)
    {
        var request = new RunnerSessionRequest(
            _profile,
            (long)SessionTtl.TotalMilliseconds);

        using var response = await _httpClient.PostAsJsonAsync(
            requestUri,
            request,
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private sealed record RunnerSessionRequest(
        RunnerProfile Profile,
        long HeartbeatTimeoutMilliseconds);
}
