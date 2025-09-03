using Pipelines.Models.Sessions;
using Pipelines.Session;

namespace Pipelines.Services;

public class SessionService(ISessionManager sessionManager)
{
    public async Task<IEnumerable<UserSessionResponse>> ListAsync(Guid userId,string sessionToken)
    {
        var sessions = await sessionManager.ListSessionsAsync(userId);

        var result = sessions.Select(x => MapToResponse(x, sessionToken))
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.LastActiveAt);
        
        return result;
    }

    private UserSessionResponse MapToResponse(UserSession session,string sessionToken)
    {

        return new UserSessionResponse
        {
            Id = session.Id,
            UserId = session.UserId,
            IsCurrent = session.SessionToken.Equals(sessionToken),
            Status = session.Status,
            CreatedAt = session.CreatedAt,
            ExpiresAt = session.ExpiresAt,
            LastActiveAt = session.LastActiveAt,
            IpAddress = session.IpAddress,
            UserAgent = session.UserAgent,
            DeviceType = session.DeviceType,
            DeviceName = session.DeviceName,
            Location = session.Location,
            TerminatedAt = session.TerminatedAt,
            TerminationReason = session.TerminationReason,
        };
    }
}
