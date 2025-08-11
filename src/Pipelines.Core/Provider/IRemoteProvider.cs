namespace Pipelines.Core.Provider;
public interface IRemoteProvider
{
    Task<RepositoryList> ListAsync(CancellationToken cancellationToken = default);
}