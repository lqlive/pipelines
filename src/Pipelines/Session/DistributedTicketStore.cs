using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Pipelines.Session;

public class DistributedTicketStore : ITicketStore
{
    private const string KeyPrefix = "Pipelines.Session-";
    private readonly IDistributedCache _cache;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<DistributedTicketStore> _logger;
    private readonly TicketSerializer _ticketSerializer;

    public DistributedTicketStore(IDistributedCache cache,
        ISessionManager sessionManager,
        ILogger<DistributedTicketStore> logger)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(sessionManager);
        ArgumentNullException.ThrowIfNull(logger);

        _cache = cache;
        _sessionManager = sessionManager;
        _logger = logger;
        _ticketSerializer = TicketSerializer.Default;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = $"{KeyPrefix}{Guid.NewGuid()}";
        await RenewAsync(key, ticket);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {

        ticket.Properties.StoreTokens([new AuthenticationToken { Name = "session_token", Value = key }]);
        var ticketBytes = _ticketSerializer.Serialize(ticket);

        var options = new DistributedCacheEntryOptions();
        var expiresUtc = ticket.Properties.ExpiresUtc;

        if (expiresUtc.HasValue)
        {
            options.SetAbsoluteExpiration(expiresUtc.Value);
        }

        if (ticket.Properties.AllowRefresh.GetValueOrDefault(false))
        {
            options.SetSlidingExpiration(TimeSpan.FromHours(1));
        }

        await _cache.SetAsync(key, ticketBytes, options);
     
        _logger.LogDebug("Ticket stored/renewed: {Key}", key);
    }

    public Task RemoveAsync(string key)
    {
        _logger.LogDebug("This method hasn't been implemented yet");
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(string key, HttpContext httpContext, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(key);

    

        _logger.LogDebug("Ticket removed: {Key}", key);
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
}