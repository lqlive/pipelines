namespace Pipelines.Models.Users;

public class LoginWithRequest
{
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public string? Avatar { get; set; }
}