using Microsoft.EntityFrameworkCore;
using Pipelines.Core.Entities;
using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Stores;

namespace Pipelines.Storage.PostgreSQL.Stores;
public class RepositoryStore(IContext context) : IRepositoryStore
{
    public async Task<bool> CreateAsync(Repository repository, CancellationToken cancellationToken)
    {
        try
        {
            context.Repositories.Add(repository);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (DbUpdateException ex)
            when (context.IsUniqueConstraintViolationException(ex))
        {
            return false;
        }
    }
}