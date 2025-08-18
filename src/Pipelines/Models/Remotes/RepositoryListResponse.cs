using Pipelines.Core.Provider;

namespace Pipelines.Models.Remotes;

public class RepositoryListResponse
{
    public int Count { get; set; }
    public IEnumerable<RepositoryItem> Items { get; set; } = new List<RepositoryItem>();
    public IEnumerable<RepositoryItem> EnabledItems { get; set; } = new List<RepositoryItem>();
}