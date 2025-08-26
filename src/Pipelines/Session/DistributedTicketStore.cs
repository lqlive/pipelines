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

        if (ticket.Properties.AllowRefresh.GetValueOrDefault(false))
        {
            options.SetSlidingExpiration(TimeSpan.FromMinutes(1));
        }

        var userIdClaim = ticket.Principal?.FindFirst("sub")?.Value;

        await _cache.SetAsync(key, ticketBytes, options);

        if (!string.IsNullOrEmpty(userIdClaim))
        {
            await _sessionManager.AddSessionAsync(new UserSession
            {
                Id = Guid.NewGuid(),
                SessionToken = key,
                UserId = Guid.Parse(userIdClaim ?? string.Empty)
            });
        }
        _logger.LogDebug("Ticket stored/renewed: {Key}", key);
    }

    public Task RemoveAsync(string key)
    {
        _logger.LogDebug("This method hasn't been implemented yet");
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(string key, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContext.User?.FindFirst("sub")?.Value;

        await _cache.RemoveAsync(key);

        if (!string.IsNullOrEmpty(userIdClaim))
        {
            await _sessionManager.RemoveSessionAsync(Guid.Parse(userIdClaim ?? string.Empty), key);
        }

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