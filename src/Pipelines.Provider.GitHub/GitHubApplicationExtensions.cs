using Microsoft.Extensions.DependencyInjection;
using Pipelines.Core;

namespace Pipelines.Provider.GitHub;
public static class GitHubApplicationExtensions
{
    public static PipelinesApplication AddGitHub(this PipelinesApplication app)
    {
        app.Services.AddScoped<GitHubClientBuilder>();

        app.Services.AddScoped<GitHubProvider>();
        
        return app;
    }
    public static PipelinesApplication AddGitHub(
    this PipelinesApplication app,
    Action<GitHubRemoteOptions> configure)
    {
        app.AddGitHub();
        app.Services.Configure(configure);
        return app;
    }
}
