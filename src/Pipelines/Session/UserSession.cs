namespace Pipelines.Session;

public interface ISession
{
    Guid Id { get; set; }
    Guid UserId { get; set; }
    string SessionToken { get; set; }
    SessionStatus Status { get; set; }
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset ExpiresAt { get; set; }
    DateTimeOffset LastActiveAt { get; set; }
}

public class UserSession : ISession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string SessionToken { get; set; }

    public SessionStatus Status { get; set; } = SessionStatus.Active;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset LastActiveAt { get; set; } = DateTimeOffset.UtcNow;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceType { get; set; }

    public string? DeviceName { get; set; }

    public string? Location { get; set; }

    public DateTimeOffset? TerminatedAt { get; set; }

    public string? TerminationReason { get; set; }

}