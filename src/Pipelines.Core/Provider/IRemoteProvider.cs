namespace Pipelines.Core.Provider;
public interface IRemoteProvider
{
    Task<RepositoryList> ListAsync(Guid userId, CancellationToken cancellationToken = default);
}