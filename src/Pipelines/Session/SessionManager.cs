using StackExchange.Redis;
using System.Text.Json;

namespace Pipelines.Session;
public class SessionManager : ISessionManager
{
    private const string UserSessionsKeyPrefix = "user_sessions:";
    private readonly IDatabase _database;
    private readonly ILogger<SessionManager> _logger;

    public SessionManager(IConnectionMultiplexer redis, ILogger<SessionManager> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<bool> AddSessionAsync(UserSession session)
    {
        try
        {
            if (string.IsNullOrEmpty(session.SessionToken) || session.UserId == Guid.Empty)
            {
                _logger.LogWarning("Invalid session data: SessionToken or UserId is empty");
                return false;
            }

            if (session.ExpiresAt == default)
            {
                session.ExpiresAt = DateTimeOffset.UtcNow.AddHours(24);
            }

            var userSessionsKey = GetUserSessionsKey(session.UserId);
            var sessionJson = JsonSerializer.Serialize(session);
            var timestamp = session.CreatedAt.ToUnixTimeSeconds();

            var transaction = _database.CreateTransaction();

            _ = transaction.SortedSetAddAsync(userSessionsKey, sessionJson, timestamp);
            _ = transaction.SortedSetRemoveRangeByRankAsync(userSessionsKey, 0, -21);
            _ = transaction.KeyExpireAsync(userSessionsKey, TimeSpan.FromDays(30));

            bool committed = await transaction.ExecuteAsync();

            if (committed)
            {
                _logger.LogInformation("Session {SessionId} added successfully for user {UserId}",
                    session.Id, session.UserId);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to commit session transaction for user {UserId}", session.UserId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add session {SessionId} for user {UserId}",
                session.Id, session.UserId);
            return false;
        }
    }

    public async Task<IEnumerable<UserSession>> ListSessionsAsync(Guid userId)
    {
        try
        {
            var userSessionsKey = GetUserSessionsKey(userId);

            var sessionEntries = await _database.SortedSetRangeByScoreWithScoresAsync(
                userSessionsKey, order: Order.Descending);

            if (!sessionEntries.Any())
            {
                _logger.LogDebug("No sessions found for user {UserId}", userId);
                return Enumerable.Empty<UserSession>();
            }

            var sessions = new List<UserSession>();
            var expiredSessions = new List<RedisValue>();

            foreach (var entry in sessionEntries)
            {
                try
                {
                    var sessionJson = entry.Element.ToString();
                    var session = JsonSerializer.Deserialize<UserSession>(sessionJson);

                    if (session != null)
                    {
                        if (session.ExpiresAt > DateTimeOffset.UtcNow &&
                            session.Status == SessionStatus.Active)
                        {
                            sessions.Add(session);
                        }
                        else
                        {
                            expiredSessions.Add(entry.Element);
                        }
                    }
                    else
                    {
                        expiredSessions.Add(entry.Element);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize session for user {UserId}", userId);
                    expiredSessions.Add(entry.Element);
                }
            }

            if (expiredSessions.Any())
            {
                await CleanupExpiredSessionsAsync(userId, expiredSessions);
            }

            _logger.LogDebug("Retrieved {Count} active sessions for user {UserId}",
                sessions.Count, userId);

            return sessions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list sessions for user {UserId}", userId);
            return Enumerable.Empty<UserSession>();
        }
    }
    public async Task<bool> RemoveSessionAsync(Guid userId, string sessionToken)
    {
        try
        {
            var userSessionsKey = GetUserSessionsKey(userId);
            var sessionEntries = await _database.SortedSetRangeByScoreWithScoresAsync(userSessionsKey);

            foreach (var entry in sessionEntries)
            {
                try
                {
                    var session = JsonSerializer.Deserialize<UserSession>(entry.Element.ToString());
                    if (session?.SessionToken == sessionToken)
                    {
                        // Remove from sorted set (user sessions list)
                        var removed = await _database.SortedSetRemoveAsync(userSessionsKey, entry.Element);
                        
                        // Remove session token from Redis cache
                        await _database.KeyDeleteAsync(sessionToken);
                        _logger.LogDebug("Session token {SessionToken} deleted from Redis cache", sessionToken);

                        if (removed)
                        {
                            _logger.LogInformation("Session {SessionToken} removed successfully for user {UserId}",
                                sessionToken, userId);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process session during removal for user {UserId}", userId);
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove session {SessionToken} for user {UserId}", sessionToken, userId);
            return false;
        }
    }

    public async Task<bool> RevokeSessionAsync(Guid userId, Guid sessionId)
    {
        try
        {
            var userSessionsKey = GetUserSessionsKey(userId);
            var sessionEntries = await _database.SortedSetRangeByScoreWithScoresAsync(userSessionsKey);

            foreach (var entry in sessionEntries)
            {
                try
                {
                    var session = JsonSerializer.Deserialize<UserSession>(entry.Element.ToString());
                    if (session?.Id == sessionId)
                    {
                        _ = await _database.KeyDeleteAsync(session.SessionToken);
                        var removed = await _database.SortedSetRemoveAsync(userSessionsKey, entry.Element);
                        if (removed)
                        {
                            _logger.LogInformation("Session {SessionId} removed successfully for user {UserId}",
                                sessionId, userId);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process session during removal for user {UserId}", userId);
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove session {SessionId} for user {UserId}", sessionId, userId);
            return false;
        }
    }
    private static string GetUserSessionsKey(Guid userId) => $"{UserSessionsKeyPrefix}{userId}";
    private async Task CleanupExpiredSessionsAsync(Guid userId, IEnumerable<RedisValue> expiredSessions)
    {
        try
        {
            var userSessionsKey = GetUserSessionsKey(userId);
            var transaction = _database.CreateTransaction();

            foreach (var sessionJson in expiredSessions)
            {
                _ = transaction.SortedSetRemoveAsync(userSessionsKey, sessionJson);
            }

            await transaction.ExecuteAsync();

            _logger.LogDebug("Cleaned up {Count} expired sessions for user {UserId}",
                expiredSessions.Count(), userId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup expired sessions for user {UserId}", userId);
        }
    }
}