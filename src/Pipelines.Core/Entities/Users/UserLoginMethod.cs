
namespace Pipelines.Core.Entities.Users;
public class UserLoginMethod
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public LoginMethod LoginMethod { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? TokenExpiresAt { get; set; }
    public bool IsPrimary { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public User User { get; set; } = null!;
}