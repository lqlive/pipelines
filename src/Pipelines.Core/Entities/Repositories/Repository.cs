namespace Pipelines.Core.Entities.Repositories;

public class Repository
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string RawId { get; set; }
    public string? Branch { get; set; }
    public string? HtmlUrl { get; set; }
    public string? SshUrl { get; set; }
    public required string CloneUrl { get; set; }
    public bool Active { get; set; }
    public string? Description { get; set; }
    public GitProvider Provider { get; set; }
    public Guid? WebhooksId { get; set; }
    public RepositoryWebhooks? Webhooks { get; set; }
    public List<RepositoryEnvironment>? Environments { get; set; }
    public RepositoryAccess? Access { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}