using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pipelines.Core.Entities;

namespace Pipelines.Database.PostgreSQL;
public class PostgreSQLContext : AbstractContext<PostgreSQLContext>
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=123456;Database=pipelines");
    }
    public override bool IsUniqueConstraintViolationException(DbUpdateException exception)
    {
        throw new NotImplementedException();
    }
}

public class PostgreSQLContextDesignFactory : IDesignTimeDbContextFactory<PostgreSQLContext>
{
    public PostgreSQLContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PostgreSQLContext>();
        var migrationsAssembly = typeof(PostgreSQLContextDesignFactory).GetTypeInfo().Assembly.GetName().Name;
        const string connectionStrings = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=pipelines";
        optionsBuilder.UseNpgsql(connectionStrings,
            sqlOptions => { sqlOptions.MigrationsAssembly(migrationsAssembly); });
        return new PostgreSQLContext();
    }
}
