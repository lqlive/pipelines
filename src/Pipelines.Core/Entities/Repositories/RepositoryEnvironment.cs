namespace Pipelines.Core.Entities.Repositories;
public class RepositoryEnvironment
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public required Repository Repository { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public bool IsSecret { get; set; }
}