using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Octokit;
using Pipelines.Core.Provider;

namespace Pipelines.Provider.GitHub;
public class GithubProvider : IRemoteProvider
{
    private readonly GitHubClient _gitHubClient;
    private readonly GitHubRemoteOptions _options;
    public GithubProvider(GitHubClient gitHubClient, IOptionsMonitor<GitHubRemoteOptions> options)
    {
        _gitHubClient = gitHubClient;
        _options = options.CurrentValue;
    }

    public Task<string> GetChallengeUrlAsync(AuthenticationProperties properties,string redirectUri, CancellationToken cancellationToken)
    {
        var queryStrings = new Dictionary<string, string>
            {
                { "client_id", _options.ClientId },
                { "response_type", "code" },
                { "redirect_uri", redirectUri }
            };

        AddQueryString(queryStrings, properties, "scope", FormatScope, _options.Scope);

        var url= QueryHelpers.AddQueryString(_options.AuthorizationEndpoint, queryStrings!);
        return Task.FromResult(url);
    }

    public async Task ListAsync(CancellationToken cancellationToken)
    {
        var repositories = await _gitHubClient.Repository.GetAllForCurrent();
        
        
        throw new NotImplementedException();
    }

    private static void AddQueryString<T>(
       Dictionary<string, string> queryStrings,
       AuthenticationProperties properties,
       string name,
       Func<T, string> formatter,
       T defaultValue)
    {
        string? value;
        var parameterValue = properties.GetParameter<T>(name);
        if (parameterValue != null)
        {
            value = formatter(parameterValue);
        }
        else if (!properties.Items.TryGetValue(name, out value))
        {
            value = formatter(defaultValue);
        }

        // Remove the parameter from AuthenticationProperties so it won't be serialized into the state
        properties.Items.Remove(name);

        if (value != null)
        {
            queryStrings[name] = value;
        }
    }

    private static void AddQueryString(
        Dictionary<string, string> queryStrings,
        AuthenticationProperties properties,
        string name,
        string? defaultValue = null)
        => AddQueryString(queryStrings, properties, name, x => x!, defaultValue);
    private string FormatScope(IEnumerable<string> scopes)
      => string.Join(" ", scopes);
}