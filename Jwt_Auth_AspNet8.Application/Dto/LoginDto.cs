using System.ComponentModel.DataAnnotations;

namespace Jwt_Auth_AspNet8.Application.Dto;

public class LoginDto
{
    [Required (ErrorMessage = "Username is required")]
    public required string UserName { get; set; }
    [Required (ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}