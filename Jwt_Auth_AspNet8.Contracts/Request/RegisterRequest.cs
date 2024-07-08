using System.ComponentModel.DataAnnotations;

namespace Jwt_Auth_AspNet8.Contracts.Requests;

public class RegisterRequest
{
    [Required (ErrorMessage = "Username is required")]
    public required string UserName { get; set; }
    [Required (ErrorMessage = "Email is required")]
    public required string Email { get; set; }
    [Required (ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}