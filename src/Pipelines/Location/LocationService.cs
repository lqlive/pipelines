using MaxMind.GeoIP2;

namespace Pipelines.Location;

public class LocationService : ILocationService
{
    public Task<Location> GetAsync(string ipAddress)
    {
        using var reader = new DatabaseReader("Assets/GeoLite2-City.mmdb");

        var city = reader.City(ipAddress);
        return Task.FromResult(new Location
        {
            Country = city.Country.Name,
            City = city.City.Name,
        });
    }
}
