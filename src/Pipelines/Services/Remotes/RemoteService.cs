using Microsoft.AspNetCore.Authentication;
using Pipelines.Provider.GitHub;

namespace Pipelines.Services.Remotes;

public class RemoteService(GithubProvider remoteProvider)
{
    public Task<string> GetChallengeUrlAsync(HttpContext context, AuthenticationProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return remoteProvider.GetChallengeUrlAsync(context, properties, cancellationToken);
    }
    public Task<AuthenticationTicket> CreateTicketAsync(string code, AuthenticationProperties properties, CancellationToken cancellationToken = default)
    {
        return remoteProvider.CreateTicketAsync(code, properties, cancellationToken);
    }
}
