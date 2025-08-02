using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Pipelines.Session;

public class DistributedTicketStore : ITicketStore
{
    private const string KeyPrefix = "Pipelines.Session-";
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedTicketStore> _logger;
    private readonly TicketSerializer _ticketSerializer;

    public DistributedTicketStore(IDistributedCache cache, ILogger<DistributedTicketStore> logger)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(logger);

        _cache = cache;
        _logger = logger;
        _ticketSerializer = TicketSerializer.Default;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var guid = Guid.NewGuid();
        var key = KeyPrefix + guid.ToString();
        await RenewAsync(key, ticket);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var ticketBytes = _ticketSerializer.Serialize(ticket);

        var options = new DistributedCacheEntryOptions();

        var expiresUtc = ticket.Properties.ExpiresUtc;
        if (expiresUtc.HasValue)
        {
            options.SetAbsoluteExpiration(expiresUtc.Value);
        }

        options.SetSlidingExpiration(TimeSpan.FromHours(1)); 

        await _cache.SetAsync(key, ticketBytes, options);

        _logger.LogDebug("Ticket stored/renewed: {Key}", key);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var ticketBytes = await _cache.GetAsync(key);
        if (ticketBytes == null || ticketBytes.Length == 0)
        {
            return null;
        }

        return _ticketSerializer.Deserialize(ticketBytes);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        _logger.LogDebug("Ticket removed: {Key}", key);
    }
}