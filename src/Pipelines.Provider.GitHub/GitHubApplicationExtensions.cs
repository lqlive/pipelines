using Microsoft.Extensions.DependencyInjection;
using Pipelines.Core;

namespace Pipelines.Provider.GitHub;
public static class GitHubApplicationExtensions
{
    public static PipelinesApplicationn AddGitHub(this PipelinesApplicationn app)
    {
        app.Services.AddScoped<GitHubClientBuilder>();

        app.Services.AddScoped<GithubProvider>();
        
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
