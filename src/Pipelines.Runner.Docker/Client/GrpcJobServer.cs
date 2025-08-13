using Grpc.Net.Client;
using Pipelines.Core.Runner;
using Pipelines.Proto;

namespace Pipelines.Runner.Docker.Client;

public class GrpcJobServer : IJobServer
{
    private readonly LogService.LogServiceClient _logClient;

    public GrpcJobServer()
    {
        var baseUrl = Environment.GetEnvironmentVariable("SCHEDULER_SERVER") ?? "http://localhost:5170";
        var channel = GrpcChannel.ForAddress(baseUrl);
        _logClient = new LogService.LogServiceClient(channel);
    }

    public async Task AppendLogAsync(Guid buildId, Guid? stepId, string content, CancellationToken cancellationToken = default)
    {
        using var call = _logClient.StreamLogs(cancellationToken: cancellationToken);
        await call.RequestStream.WriteAsync(new LogChunk
        {
            BuildId = buildId.ToString(),
            StepId = stepId?.ToString() ?? string.Empty,
            Content = content
        });
        await call.RequestStream.CompleteAsync();
        await call.ResponseAsync;
    }

    public Task<bool> IsCancellationRequestedAsync(Guid buildId, CancellationToken cancellationToken = default)
    {
        // TODO: add a cancel RPC; fallback to false for now
        return Task.FromResult(false);
    }
}


