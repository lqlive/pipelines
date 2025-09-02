using Pipelines.Models.Sessions;
using Pipelines.Session;

namespace Pipelines.Services;

public class SessionService(ISessionManager sessionManager)
{
    public async Task<IEnumerable<UserSessionResponse>> ListAsync(Guid userId)
    {
        var sessions = await sessionManager.ListSessionsAsync(userId);

        var result = sessions.Select(x => MapToResponse(x, string.Empty));
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
