namespace Pipelines.Core.Entities.Users;

public class UserSetting
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "light";
    public EmailNotification EmailNotifications { get; set; } = EmailNotification.FailuresOnly;
    public bool EnableBrowserNotifications { get; set; } = true;
    public bool EnableTwoFactorAuthentication { get; set; } = false;
    public int MaxConcurrentBuilds { get; set; } = 10;
    public int BuildTimeoutMinutes { get; set; } = 60;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public User User { get; set; } = null!;
}