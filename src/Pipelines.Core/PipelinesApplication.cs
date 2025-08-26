using Microsoft.Extensions.DependencyInjection;

namespace Pipelines.Core;
public class PipelinesApplication(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}