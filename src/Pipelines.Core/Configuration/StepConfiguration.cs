namespace Pipelines.Core.Configuration;

public class StepConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public List<string> Commands { get; set; } = new();
    public List<string>? Command { get; set; }
    public List<string>? Entrypoint { get; set; }

    public Dictionary<string, string> Environment { get; set; } = new();
    public List<VolumeConfiguration> Volumes { get; set; } = new();
    public string? WorkingDirectory { get; set; }
    public string? User { get; set; }
    public string? NetworkMode { get; set; }

    public bool Privileged { get; set; } = false;
    public bool Detach { get; set; } = false;
    public PullPolicy Pull { get; set; } = PullPolicy.IfNotExists;
    public FailurePolicy Failure { get; set; } = FailurePolicy.Always;

    public int? TimeoutMinutes { get; set; }
    public List<string> DependsOn { get; set; } = new();
    public TriggerConfiguration? When { get; set; }
}

public enum PullPolicy
{
    Always,
    Never,
    IfNotExists
}

public enum FailurePolicy
{
    Always,
    Ignore
}