using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Provider;
using Pipelines.Core.Stores;
using Pipelines.Errors;
using Pipelines.Models.Remotes;
using Pipelines.Provider.GitHub;

namespace Pipelines.Services.Remotes;

public class RemoteService(GitHubProvider remoteProvider,
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
    public async Task<ErrorOr<RepositoryListResponse>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await remoteProvider.ListAsync(userId, cancellationToken);
            var enabledItems = await repositoryStore.ListAsync(cancellationToken);

            var result = new RepositoryListResponse
            {
                Items = items.Items.Select(x => new RepositoryItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    CloneUrl = x.CloneUrl,
                    Description = x.Description,
                }).ToList(),

                EnabledItems = enabledItems.Select(x => new RepositoryItem
                {
                    Id = long.Parse(x.RawId),
                    Name = x.Name,
                    CloneUrl = x.CloneUrl,
                    Description = x.Description,
                }).ToList(),
                Count = items.Count
            };
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "");
            return RemoteErrors.Unauthorized;
        }
    }

    public async Task<ErrorOr<Success>> EnableAsync(Guid userId, EnableRepositoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var repositoryList = await remoteProvider.ListAsync(userId, cancellationToken);

            foreach (var id in request.Ids ?? [])
            {
                var repository = repositoryList.Items.SingleOrDefault(x => x.Id == id);
                if (repository is null)
                {
                    continue;
                }

                var newRepository = new Repository
                {
                    RawId = repository.Id.ToString(),
                    Name = repository.Name,
                    CloneUrl = repository.CloneUrl,
                    Description = repository.Description,
                    Provider = GitProvider.GitHub
                };
                await repositoryStore.CreateAsync(newRepository, cancellationToken);
            }
 
            return Result.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "");
            return RemoteErrors.Unauthorized;
        }
    }
}