using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Entities.Users;

namespace Pipelines.Core.Entities;
public interface IContext
{
    DatabaseFacade Database { get; }
    DbSet<Build> Builds { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Repository> Repositories { get; set; }
    bool IsUniqueConstraintViolationException(DbUpdateException exception);
    bool SupportsLimitInSubqueries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task RunMigrationsAsync(CancellationToken cancellationToken);
}
