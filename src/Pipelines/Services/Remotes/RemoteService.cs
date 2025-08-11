using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Pipelines.Core.Provider;
using Pipelines.Errors;
using Pipelines.Provider.GitHub;

namespace Pipelines.Services.Remotes;

public class RemoteService(GithubProvider remoteProvider,ILogger<RemoteService> logger)
{
    public Task<string> GetChallengeUrlAsync(HttpContext context, AuthenticationProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return remoteProvider.GetChallengeUrlAsync(context, properties, cancellationToken);
    }
    public Task<AuthenticationTicket> CreateTicketAsync(string code, AuthenticationProperties properties, CancellationToken cancellationToken = default)
    {
        return remoteProvider.CreateTicketAsync(code, properties, cancellationToken);
    }

    public async Task<ErrorOr<RepositoryList>> ListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result =  await remoteProvider.ListAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "");
            return RemoteErrors.Unauthorized;
        } 
    }
}
