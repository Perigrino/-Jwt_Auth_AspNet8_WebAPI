
using Jwt_Auth_AspNet8.Application.Dto;
using Jwt_Auth_AspNet8.Application.Interfaces;
using Jwt_Auth_AspNet8.Application.Services;
using Microsoft.AspNetCore.Mvc;


namespace Jwt_Auth_AspNet8.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Route For Seeding My Roles to DB
        [HttpPost(ApiEndpoints.Auth.SeedRoles)]
        public async Task<IActionResult> SeedRoles ()
        {
            var seedData = await _authService.SeedRolesAsync();
            return Ok(seedData);
        }
        
        
        // Route -> Register
        [HttpPost(ApiEndpoints.Auth.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = await _authService.RegisterAsync(registerDto);
            return Ok(user);
        }
        
        // Route -> Login
        [HttpPost(ApiEndpoints.Auth.Login)]
        public async Task<IActionResult> Login ([FromBody] LoginDto loginRequest)
        {
            var userLogin = await _authService.LoginAsync(loginRequest);
            return Ok(userLogin);
        }
        
        
        // Route -> Make User -> ADMIN
        [HttpPost(ApiEndpoints.Auth.MakeAdmin)]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatePermissionDto updatePermissionRequest)
        {
            var user = await _authService.MakeAdminAsync(updatePermissionRequest);
            return Ok(user);
        }
        
        
        // Route -> Make User -> Owner
        [HttpPost(ApiEndpoints.Auth.MakeOwner)]
        public async Task<IActionResult> MakeOwner([FromBody] UpdatePermissionDto updatePermissionRequest)
        {
            var user = await _authService.MakeOwnerAsync(updatePermissionRequest);
            return Ok(user);
        }
        
        
        // Route -> Remove Owner Role
        [HttpPost(ApiEndpoints.Auth.RemoveOwnerRole)]
        public async Task<IActionResult> RemoveOwnerRole([FromBody] UpdatePermissionDto updatePermissionRequest)
        {
            var user = await _authService.RemoveOwnerRoleAsync(updatePermissionRequest);
            return Ok(user);
        }
        
        
        // Route -> Remove Admin Role
        [HttpPost(ApiEndpoints.Auth.RemoveAdminRole)]
        public async Task<IActionResult> RemoveAdminRole([FromBody] UpdatePermissionDto updatePermissionRequest)
        {
            var user = await _authService.RemoveAdminRoleAsync(updatePermissionRequest);
            return Ok(user);
        }
    }
}