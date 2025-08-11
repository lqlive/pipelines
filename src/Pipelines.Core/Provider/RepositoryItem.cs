namespace Pipelines.Core.Provider;
public class RepositoryItem
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public string? Description { get; set; }
}
