namespace Pipelines.Core.Entities.Repositories;

public class Repository
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public GitProvider Provider { get; set; }
}