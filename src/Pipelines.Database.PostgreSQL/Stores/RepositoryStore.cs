using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Pipelines.Core.Entities;
using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Stores;

namespace Pipelines.Database.PostgreSQL.Stores;
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

    public async Task<Repository?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var repository = await context.Repositories
            .Include(x => x.Webhooks)
            .Include(x => x.Environments)
            .SingleOrDefaultAsync(x => x.Id == id);
     
        return repository;
    }

    public async Task<bool> UpdateAsync(Repository repository, CancellationToken cancellationToken)
    {
        try
        {
            context.Repositories.Update(repository);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex)
            when (context.IsUniqueConstraintViolationException(ex))
        {
            return false;
        }
    }

    public async Task<IEnumerable<Repository>> ListAsync(CancellationToken cancellationToken)
    {
        return await context.Repositories.ToListAsync();
    }

    public async Task<bool> DeleteAsync(Repository repository, CancellationToken cancellationToken)
    {
        try
        {
            context.Repositories.Remove(repository);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbException)
        {
            return false;
        }
    }
}