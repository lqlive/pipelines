using ErrorOr;

using Microsoft.AspNetCore.Authentication;

using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Provider;
using Pipelines.Core.Stores;
using Pipelines.Errors;
using Pipelines.Provider.GitHub;

namespace Pipelines.Services.Remotes;

public class RemoteService(GithubProvider remoteProvider,
    IUserStore userStore,
    IRepositoryStore repositoryStore,
    ILogger<RemoteService> logger)
{
    public Task<string> GetChallengeUrlAsync(HttpContext context, AuthenticationProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return remoteProvider.GetChallengeUrlAsync(context, properties, cancellationToken);
    }
    public Task<AuthenticationTicket> CreateTicketAsync(string code, AuthenticationProperties properties, CancellationToken cancellationToken = default)
    {
        return remoteProvider.CreateTicketAsync(code, properties, cancellationToken);
    }

    public async Task<ErrorOr<Success>> CallbackAsync(Guid userId, string accessToken, CancellationToken cancellationToken = default)
    {
        var user = await userStore.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return RemoteErrors.Unauthorized;
        }

        if (!user.Providers?.Any(x => x.Name == "GitHub") ?? false)
        {
            var provider = new UserProvider
            {
                Name = "GitHub",
                AccessToken = accessToken
            };
            await userStore.CreateProviderAsync(user, provider, cancellationToken);
        }

        return Result.Success;
    }
    public async Task<ErrorOr<RepositoryList>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await remoteProvider.ListAsync(userId, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "");
            return RemoteErrors.Unauthorized;
        }
    }

    public async Task<ErrorOr<Success>> EnableAsync(Guid userId, long repositoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var repository = await remoteProvider.GetAsync(userId, repositoryId, cancellationToken);
            var newRepository = new Repository
            {
                RawId = repository.Id,
                Name = repository.Name,
                Url = repository.Url,
                Provider = GitProvider.GitHub
            };

            await repositoryStore.CreateAsync(newRepository, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "");
            return RemoteErrors.Unauthorized;
        }
    }
}