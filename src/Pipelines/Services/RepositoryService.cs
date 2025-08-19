using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Stores;
using Pipelines.Models.Repositories;

namespace Pipelines.Services;

public class RepositoryService(IRepositoryStore repositoryStore)
{

    public async Task<IEnumerable<RepositoryResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var repositories = await repositoryStore.ListAsync(cancellationToken);

        return repositories.Select(MapToResponse);
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

