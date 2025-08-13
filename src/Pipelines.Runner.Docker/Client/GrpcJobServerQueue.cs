using Grpc.Net.Client;
using Pipelines.Core.Runner;
using Pipelines.Proto;
using Pipelines.Runner.Docker.Services;

namespace Pipelines.Runner.Docker.Client;

public class GrpcJobServerQueue : IJobServerQueue
{
    private readonly RunnerService.RunnerServiceClient _client;
    private readonly RunnerRegistrationService _registration;

    public GrpcJobServerQueue(RunnerRegistrationService registration)
    {
        var baseUrl = Environment.GetEnvironmentVariable("SCHEDULER_SERVER") ?? "http://localhost:5170";
        var channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler()
        });
        _client = new RunnerService.RunnerServiceClient(channel);
        _registration = registration;
    }

    public Task EnqueueAsync(object job, CancellationToken cancellationToken = default)
    {
        // Scheduling remains via Pipelines API; this queue only handles dequeue via gRPC.
        return Task.CompletedTask;
    }

    public async Task<T?> DequeueAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        var runnerId = _registration.RunnerId;
        if (string.IsNullOrEmpty(runnerId)) return null;

        var resp = await _client.RequestJobAsync(new JobRequest { RunnerId = runnerId }, cancellationToken: cancellationToken);
        if (!resp.HasJob) return null;

        // naive map back to Build JSON via System.Text.Json
        var json = System.Text.Json.JsonSerializer.Serialize(new
        {
            id = resp.Build.Id,
            repository = resp.Build.Repository,
            timeoutSeconds = resp.Build.TimeoutSeconds,
            steps = resp.Build.Steps.Select(s => new { id = s.Id, image = s.Image, script = s.Script })
        });
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }
}


