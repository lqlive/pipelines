namespace Pipelines.Session;

public class UserSession 
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