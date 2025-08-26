using Microsoft.Extensions.DependencyInjection;

namespace Pipelines.Core.Management;
public static class PipelinesApplicationExtensions
{
    public static PipelinesApplication AddPipelinesCore(this IServiceCollection services)
    {
        var app = new PipelinesApplication(services);
        return app;
    }
}
