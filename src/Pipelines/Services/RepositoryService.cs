using ErrorOr;
using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Stores;
using Pipelines.Errors;
using Pipelines.Models.Repositories;

namespace Pipelines.Services;

public class RepositoryService(IRepositoryStore repositoryStore)
{

    public async Task<IEnumerable<RepositoryResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var repositories = await repositoryStore.ListAsync(cancellationToken);

        return repositories.Select(MapToResponse);
    }

    public async Task<ErrorOr<Success>> PatchAsync(
       Guid id,
       Patch<RepositoryRequest> patch,
       CancellationToken cancellationToken)
    {
        var repository = await repositoryStore.GetAsync(id, cancellationToken);

        if (repository is null)
        {
            return RepositoryErrors.RepositoryNotFound;
        }

        patch.ApplyTo(repository);
        repository.UpdatedAt = DateTimeOffset.UtcNow;

        await repositoryStore.UpdateAsync(repository, cancellationToken);
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteAsync(
       Guid id,
       CancellationToken cancellationToken)
    {
        var repository = await repositoryStore.GetAsync(id, cancellationToken);

        if (repository is null)
        {
            return RepositoryErrors.RepositoryNotFound;
        }

        await repositoryStore.DeleteAsync(repository, cancellationToken);
        return Result.Success;
    }

    private static RepositoryResponse MapToResponse(Repository repository)
    {
        return new RepositoryResponse
        {
            Id = repository.Id,
            Name = repository.Name,
            Description = repository.Description,
            CreatedAt = repository.CreatedAt,
            UpdatedAt = repository.UpdatedAt
        };
    }
}

