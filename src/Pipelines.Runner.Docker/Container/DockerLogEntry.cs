namespace Pipelines.Runner.Docker.Container;

public class DockerLogEntry
{
    public string Stream { get; init; } = "stdout";
    public string Text { get; init; } = string.Empty;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
