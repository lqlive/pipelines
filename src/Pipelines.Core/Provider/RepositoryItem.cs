namespace Pipelines.Core.Provider;
public class RepositoryItem
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string CloneUrl { get; set; }
    public string? Description { get; set; }
}
