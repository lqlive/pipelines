namespace Pipelines.Location;

public interface ILocationService
{
    Task<Location> GetAsync(string ipAddress);
}
