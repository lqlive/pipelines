using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Pipelines.Core.Provider;
public abstract class OAuthProvider
{
    public abstract Task<string> GetChallengeUrlAsync(HttpContext context,
        AuthenticationProperties? properties = null,
        CancellationToken cancellationToken = default);

    public abstract Task<AuthenticationTicket> CreateTicketAsync(string code,
        AuthenticationProperties properties,
        CancellationToken cancellationToken = default);

    protected string BuildRedirectUri(HttpContext context, string targetPath)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{targetPath}";
    }
}