namespace Pipelines.Core.Configuration;

public class ConstraintConfiguration
{
    public List<string> Include { get; set; } = new();
    public List<string> Exclude { get; set; } = new();
}