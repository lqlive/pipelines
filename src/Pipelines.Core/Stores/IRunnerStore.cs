using Pipelines.Core.Entities;

namespace Pipelines.Core.Stores;

public interface IRunnerStore
{
    Task UpsertAsync(RunnerRecord runner, CancellationToken cancellationToken = default);

    Task<RunnerRecord?> GetAsync(Guid runnerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RunnerRecord>> ListAsync(CancellationToken cancellationToken = default);
}
