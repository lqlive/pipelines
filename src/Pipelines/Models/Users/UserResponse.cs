using Pipelines.Core.Entities.Users;

namespace Pipelines.Models.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
}
