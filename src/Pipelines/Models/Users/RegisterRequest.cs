using System.ComponentModel.DataAnnotations;

namespace Pipelines.Models.Users;
public class RegisterRequest
{
    [Required]
    public required string Name { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}
