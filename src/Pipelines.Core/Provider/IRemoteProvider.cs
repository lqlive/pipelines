namespace Pipelines.Core.Provider;
public interface IRemoteProvider
{
    Task<string> GetChallengeUrlAsync(AuthenticationProperties propertie,string redirectUri,CancellationToken cancellationToken);
    Task ListAsync(CancellationToken cancellationToken);
}
