using Octokit;
using Pipelines.Core.Stores;

namespace Pipelines.Provider.GitHub;
public  class GitHubClientBuilder
{
    private readonly IUserStore _userStore;
    
    public GitHubClientBuilder(IUserStore userStore)
    {
        _userStore = userStore;
    }
    public async Task<IGitHubClient> CreateClientAsync()
    {
        var productHeader = new ProductHeaderValue("Pipelines");
        var client = new GitHubClient(productHeader);

        _ = await _userStore.GetByEmailAsync("", CancellationToken.None);

        if (!string.IsNullOrEmpty(""))
        {
            var credentials = new Credentials("");
            client.Credentials = credentials;
        }

        return client;
    }
}