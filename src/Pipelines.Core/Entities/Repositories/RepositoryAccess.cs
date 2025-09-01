namespace Pipelines.Core.Entities.Repositories;
public class RepositoryAccess
{
    public List<string>? AdminUsers { get; set; }
    public List<string>? WriteUsers { get; set; }
    public List<string>? ReadUsers { get; set; }
}