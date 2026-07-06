using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Listener;

public sealed class TaskBrokerClient : ITaskQueue
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly HttpClient _httpClient;

    public TaskBrokerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AcquiredTask?> TryAcquireAsync(
        RunnerProfile profile,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "/tasks/acquire",
            profile,
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AcquiredTask>(
            JsonOptions,
            cancellationToken);
    }

    public async Task<bool> RenewLeaseAsync(
        Guid taskId,
        Guid runnerId,
        string leaseToken,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        var request = new RenewLeaseRequest(
            runnerId,
            leaseToken,
            (long)leaseDuration.TotalMilliseconds);

        using var response = await _httpClient.PostAsJsonAsync(
            $"/tasks/{taskId}/lease",
            request,
            JsonOptions,
            cancellationToken);

        if (response.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task CompleteAsync(
        Guid taskId,
        Guid runnerId,
        string leaseToken,
        PipelineResult result,
        CancellationToken cancellationToken = default)
    {
        var request = new CompleteTaskRequest(
            runnerId,
            leaseToken,
            result);

        using var response = await _httpClient.PostAsJsonAsync(
            $"/tasks/{taskId}/complete",
            request,
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private sealed record RenewLeaseRequest(
        Guid RunnerId,
        string LeaseToken,
        long LeaseDurationMilliseconds);

    private sealed record CompleteTaskRequest(
        Guid RunnerId,
        string LeaseToken,
        PipelineResult Result);
}