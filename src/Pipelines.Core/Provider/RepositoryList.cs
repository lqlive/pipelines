namespace Pipelines.Core.Provider;
public class RepositoryList
{
    public int Count { get; set; }
    public IEnumerable<RepositoryItem> Items { get; set; } = new List<RepositoryItem>();
}