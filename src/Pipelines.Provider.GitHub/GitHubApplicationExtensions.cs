using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Pipelines.Core;
using Pipelines.Core.Provider;

namespace Pipelines.Provider.GitHub;
public static class GitHubApplicationExtensions
{
    public static PipelinesApplicationn AddGitHub(this PipelinesApplicationn app)
    {
        app.Services.AddSingleton(provider =>
        {
            var productHeader = new ProductHeaderValue("Pipelines");
            return new GitHubClient(productHeader);
        });
        app.Services.AddTransient<GithubProvider>();
        
        return app;
    }
    public static PipelinesApplicationn AddGitHub(
    this PipelinesApplicationn app,
    Action<GitHubRemoteOptions> configure)
    {
        app.AddGitHub();
        app.Services.Configure(configure);
        return app;
    }
}
