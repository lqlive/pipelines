namespace Pipelines.Core.Provider;
public interface IRemoteProvider
{
    Task<RepositoryList> ListAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<RepositoryItem> GetAsync(Guid userId, long repositoryId, CancellationToken cancellationToken = default);
}