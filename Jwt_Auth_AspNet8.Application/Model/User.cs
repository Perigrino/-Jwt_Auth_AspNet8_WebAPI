using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Jwt_Auth_AspNet8.Application.Model;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}