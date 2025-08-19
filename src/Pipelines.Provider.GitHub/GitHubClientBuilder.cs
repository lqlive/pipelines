using Octokit;
using Pipelines.Core.Stores;

namespace Pipelines.Provider.GitHub;
public class GitHubClientBuilder
{
    private readonly IUserStore _userStore;
    public GitHubClientBuilder(IUserStore userStore)
    {
        _userStore = userStore;
    }
    public async Task<IGitHubClient> CreateClientAsync(Guid? userId = null, CancellationToken cancellation = default)
    {
        var productHeader = new ProductHeaderValue("Pipelines");
        var client = new GitHubClient(productHeader);

        if (userId is not null)
        {
            var user = await _userStore.GetByIdAsync(userId.Value, cancellation);
            var provider = user?.Providers?.SingleOrDefault(x => x.Name == "GitHub");
            var credentials = new Credentials(provider?.AccessToken);
            client.Credentials = credentials;
        }

        return client;
    }
}