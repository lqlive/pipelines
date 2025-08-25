using Microsoft.EntityFrameworkCore;

using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Entities.Repositories;
using Pipelines.Core.Entities.Users;

namespace Pipelines.Core.Entities;
public abstract class AbstractContext<TContext> : DbContext, IContext where TContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Build> Builds { get; set; }
    public DbSet<Repository> Repositories { get; set; }
    public bool SupportsLimitInSubqueries => true;

    public abstract bool IsUniqueConstraintViolationException(DbUpdateException exception);
    public virtual Task RunMigrationsAsync(CancellationToken cancellationToken)
        => Database.MigrateAsync(cancellationToken);
}