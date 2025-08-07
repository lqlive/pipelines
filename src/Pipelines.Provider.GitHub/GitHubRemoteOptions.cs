namespace Pipelines.Provider.GitHub;
public class GitHubRemoteOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public string CallbackPath { get; set; } = default!;
    public string AuthorizationEndpoint { get; set; } = default!;
    public string TokenEndpoint { get; set; } = default!;
    public ICollection<string> Scopes { get; } = new HashSet<string>();
}