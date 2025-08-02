namespace Pipelines.Core.Configuration;

public class PipelineConfiguration
{
    public string Kind { get; set; } = "pipeline";
    public string Type { get; set; } = "docker";
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";

    public PlatformConfiguration Platform { get; set; } = new();
    public WorkspaceConfiguration Workspace { get; set; } = new();
    public CloneConfiguration Clone { get; set; } = new();
    public List<StepConfiguration> Steps { get; set; } = new();
    public List<VolumeConfiguration> Volumes { get; set; } = new();
    public List<string> ImagePullSecrets { get; set; } = new();
    public TriggerConfiguration Trigger { get; set; } = new();
    public ConcurrencyConfiguration Concurrency { get; set; } = new();
    public Dictionary<string, string> Node { get; set; } = new();
}