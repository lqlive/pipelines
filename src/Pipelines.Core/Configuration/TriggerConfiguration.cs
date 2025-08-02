namespace Pipelines.Core.Configuration;

public class TriggerConfiguration
{
    public ConstraintConfiguration? Branch { get; set; }
    public ConstraintConfiguration? Event { get; set; }
    public ConstraintConfiguration? Status { get; set; }
    public ConstraintConfiguration? Ref { get; set; }
}