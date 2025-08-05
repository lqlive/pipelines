using Microsoft.Extensions.DependencyInjection;

namespace Pipelines.Core.Management;
public static class PipelinesApplicationExtensions
{
    public static PipelinesApplicationn AddPipelinesCore(this IServiceCollection services)
    {
        var app = new PipelinesApplicationn(services);
        return app;
    }
}
