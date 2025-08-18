using Pipelines.Core.Entities.Repositories;

namespace Pipelines.Core.Stores;
public interface IRepositoryStore
{
    Task<bool> CreateAsync(Repository repository, CancellationToken cancellationToken);
    Task<IEnumerable<Repository>> ListAsync(CancellationToken cancellationToken);
}
