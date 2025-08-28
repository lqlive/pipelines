namespace Pipelines.Core.Entities.Repositories;
public class RepositoryWebhooks
{
    public Guid Id { get; set; }
    public bool Enabled { get; set; }
    public required string Url { get; set; }
    public string? Secret { get; set; }
    public List<string>? Events { get; set; }
}