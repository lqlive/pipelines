namespace Pipelines.Session;

public interface ISessionManager
{
   Task<bool> AddSessionAsync(UserSession session);
   Task<IEnumerable<UserSession>> ListSessionsAsync(Guid userId);
   Task<bool> RemoveSessionAsync(Guid userId, string sessionToken);
}
