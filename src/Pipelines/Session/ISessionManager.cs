namespace Pipelines.Session;

public interface ISessionManager<ISession>
{
   Task<bool> AddSessionAsync(ISession session);
   Task<IEnumerable<ISession>> ListSessionsAsync(Guid userId);
   Task<bool> RemoveSessionAsync(Guid userId, string sessionToken);
}
