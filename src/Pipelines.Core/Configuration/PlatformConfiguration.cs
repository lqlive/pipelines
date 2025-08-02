namespace Pipelines.Core.Configuration;

public class PlatformConfiguration
{
    public string Os { get; set; } = "linux";
    public string Arch { get; set; } = "amd64";
    public string? Variant { get; set; }
    public string? Version { get; set; }
}