using Microsoft.AspNetCore.Authentication;
namespace Pipelines.Core.Provider;
public interface IRemoteProvider
{   
    Task ListAsync(CancellationToken cancellationToken = default);
}
