using System.ComponentModel.DataAnnotations;

namespace Jwt_Auth_AspNet8.Application.Dto;

public class UpdatePermissionDto
{
    [Required(ErrorMessage = "Username is required")]
    public required string UserName { get; set; }
}