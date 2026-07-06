namespace Pipelines.Runner.Docker.Container;

public sealed class DockerContainerSpec
{
    public string Name { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public string WorkingDirectory { get; init; } = "/workspace";
    public string? NetworkMode { get; init; }
    public string? User { get; init; }
    public bool Privileged { get; init; }
    public IReadOnlyList<string> Entrypoint { get; init; } = [];
    public IReadOnlyList<string> Command { get; init; } = [];
    public IReadOnlyList<DockerVolumeMount> Volumes { get; init; } = [];
    public IReadOnlyDictionary<string, string> Environment { get; init; } = new Dictionary<string, string>();
    public IReadOnlyDictionary<string, string> Labels { get; init; } = new Dictionary<string, string>();
}
