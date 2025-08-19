using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Pipelines.Core.Provider;
namespace Pipelines.Provider.GitHub;
public sealed class GitHubProvider : OAuthProvider, IRemoteProvider
{
    private readonly GitHubClientBuilder _builder;
    private readonly GitHubRemoteOptions _options;
    private readonly ILogger<GitHubProvider> _logger;
    public GitHubProvider(
        GitHubClientBuilder builder,
        IOptionsMonitor<GitHubRemoteOptions> options,
        ILogger<GitHubProvider> logger)
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
        var redirectUri = BuildRedirectUri(context, properties?.RedirectUri ?? _options.CallbackPath);
        var request = new OauthLoginRequest(_options.ClientId)
        {
            RedirectUri = new Uri(redirectUri),
        };

        var client = await _builder.CreateClientAsync();
        var uri = client.Oauth.GetGitHubLoginUrl(request);
        return uri.ToString();
    }

    public async Task<RepositoryList> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var client = await _builder.CreateClientAsync(userId, cancellationToken);

        var repositories = await client.Repository.GetAllForCurrent();

        var items = repositories.Select(MapResponse);

        return new RepositoryList
        {
            Count = items.Count(),
            Items = items
        };
    }

    public async Task<RepositoryItem> GetAsync(Guid userId, long repositoryId, CancellationToken cancellationToken = default)
    {
        var client = await _builder.CreateClientAsync(userId, cancellationToken);
        var repository = await client.Repository.Get(repositoryId);
        return MapResponse(repository);
    }

    private RepositoryItem MapResponse(Repository repository)
    {
        return new RepositoryItem
        {
            Id = repository.Id,
            Name = repository.Name,
            CloneUrl = repository.CloneUrl,
            Description = repository.Description,
        };
    }
}