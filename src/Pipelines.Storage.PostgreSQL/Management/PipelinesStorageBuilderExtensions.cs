using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pipelines.Core;
using Pipelines.Core.Entities;
using Pipelines.Core.Stores;
using Pipelines.Storage.PostgreSQL.Stores;

namespace Pipelines.Storage.PostgreSQL.Management;
public static class PipelinesStorageBuilderExtensions
{
    public static PipelinesApplicationn AddPostgreSQLDatabase(this PipelinesApplicationn application)
    {
        var services = application.Services;

        services.TryAddScoped<IContext>(provider => provider.GetRequiredService<PostgreSQLContext>());

        services.AddDbContext<PostgreSQLContext>();

        services.TryAddTransient<IUserStore, UserStore>();
        return application;
    }
}
