using System.ComponentModel.DataAnnotations;

namespace Jwt_Auth_AspNet8.Contracts.Requests;

public class UpdatePermissionRequest
{
    [Required(ErrorMessage = "Username is required")]
    public required string UserName { get; set; }
}