namespace Pipelines.Runner.Docker.Container;

public sealed class DockerVolumeMount
{
    public string Source { get; init; } = string.Empty;
    public string Target { get; init; } = string.Empty;
    public bool ReadOnly { get; init; }
    public DockerVolumeMountType Type { get; init; } = DockerVolumeMountType.Bind;
}

public enum DockerVolumeMountType
{
    Bind,
    Volume,
    Tmpfs
}