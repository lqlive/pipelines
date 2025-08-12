namespace Pipelines.Core.Entities.Repositories;

public class Repository
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string RawId { get; set; }
    public required string Url { get; set; }
    public GitProvider Provider { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}