namespace Pipelines.Core.Models;
public sealed class JobRequest
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
}
