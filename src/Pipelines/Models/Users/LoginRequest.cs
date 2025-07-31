using System.ComponentModel.DataAnnotations;

namespace Pipelines.Models.Users;

public class LoginRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}