using Pipelines.Core.Entities.Repositories;

namespace Pipelines.Core.Stores;
public interface IRepositoryStore
{
    Task<Repository?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Repository repository, CancellationToken cancellationToken);
    Task<bool> CreateAsync(Repository repository, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Repository repository, CancellationToken cancellationToken);
    Task<IEnumerable<Repository>> ListAsync(CancellationToken cancellationToken);
}
