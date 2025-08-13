using System.Threading.Tasks;
using Grpc.Core;
using Pipelines.Proto;
using Pipelines.Services.Builds;

namespace Pipelines.Runner.Listener.Services;

public class GrpcLogService : LogService.LogServiceBase
{
    private readonly LogStorageService _logStorage;

    public GrpcLogService(LogStorageService logStorage)
    {
        _logStorage = logStorage;
    }

    public override async Task<LogAck> StreamLogs(IAsyncStreamReader<LogChunk> requestStream, ServerCallContext context)
    {
        await foreach (var chunk in requestStream.ReadAllAsync(context.CancellationToken))
        {
            if (!Guid.TryParse(chunk.BuildId, out var buildId))
                continue;
            Guid? stepId = Guid.TryParse(chunk.StepId, out var sid) ? sid : null;
            await _logStorage.AppendAsync(buildId, stepId, chunk.Content, context.CancellationToken);
        }
        return new LogAck();
    }
}


