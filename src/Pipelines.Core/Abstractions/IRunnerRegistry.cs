using Pipelines.Core.Entities;
using Pipelines.Core.Models;

namespace Pipelines.Abstractions;

public interface IRunnerRegistry
{
    Task<RunnerRecord> RegisterAsync(
        RunnerProfile profile,
        TimeSpan heartbeatTimeout,
        CancellationToken cancellationToken = default);

    Task<RunnerRecord> HeartbeatAsync(
        RunnerProfile profile,
        TimeSpan heartbeatTimeout,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RunnerRecord>> ListAsync(
        CancellationToken cancellationToken = default);
}
