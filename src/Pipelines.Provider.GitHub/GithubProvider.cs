using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Pipelines.Core.Provider;

namespace Pipelines.Provider.GitHub;
public sealed class GithubProvider : OAuthProvider, IRemoteProvider
{
    private readonly GitHubClientBuilder _builder;
    private readonly GitHubRemoteOptions _options;
    private readonly ILogger<GithubProvider> _logger;
    public GithubProvider(
        GitHubClientBuilder builder,
        IOptionsMonitor<GitHubRemoteOptions> options,
        ILogger<GithubProvider> logger)
    {
        _builder = builder;
        _options = options.CurrentValue;
        _logger = logger;
    }

    public override async Task<AuthenticationTicket> CreateTicketAsync(string code,
        AuthenticationProperties properties, 
        CancellationToken cancellationToken = default)
    {
        var client = await _builder.CreateClientAsync();
        var request = new OauthTokenRequest(_options.ClientId, _options.ClientSecret, code);
        var accessToken = await client.Oauth.CreateAccessToken(request, cancellationToken);
     
        if (!string.IsNullOrEmpty(accessToken.Error))
        {
            throw new AuthenticationFailureException(accessToken.ErrorDescription);
        }

        var identity = new ClaimsIdentity("GitHub");

        properties.StoreTokens(
        [
             new AuthenticationToken { Name = "access_token", Value = accessToken.AccessToken },
             new AuthenticationToken { Name = "token_type", Value = accessToken.TokenType },
        ]);

        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationTicket(principal, properties, "GitHub");
    }

    public override async Task<string> GetChallengeUrlAsync(HttpContext context,
        AuthenticationProperties? properties = null,
        CancellationToken cancellationToken = default)
    {
        var redirectUri = BuildRedirectUri(context, _options.CallbackPath);
        var request = new OauthLoginRequest(_options.ClientId)
        {
            RedirectUri = new Uri(redirectUri),
        };

        var client = await _builder.CreateClientAsync();
        var uri = client.Oauth.GetGitHubLoginUrl(request);
        return uri.ToString();
    }

    public Task ListAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}