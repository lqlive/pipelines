namespace Pipelines.Core.Runner;

public interface IBrokerServer
{
    Task ConnectAsync(Uri serverUrl);
}
