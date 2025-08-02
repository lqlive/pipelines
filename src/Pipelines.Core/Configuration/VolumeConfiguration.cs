namespace Pipelines.Core.Configuration;

public class VolumeConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public bool ReadOnly { get; set; } = false;
}