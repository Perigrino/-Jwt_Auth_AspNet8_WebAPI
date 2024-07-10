using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jwt_Auth_AspNet8.Application.Model;
using Jwt_Auth_AspNet8.Contracts.OtherObjects;
using Jwt_Auth_AspNet8.Contracts.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Jwt_Auth_AspNet8.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        
        // Route For Seeding My Roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles ()
        {
            bool ownerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool userRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
            bool adminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);

            if (adminRoleExists && userRoleExists && ownerRoleExists)
            {
                return Ok("Role Seeding Already Done");
            }
            
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

            return Ok("Role Seeding Done Successfully");
        }
        
        
        // Route -> Register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var isExistUser = await _userManager.FindByEmailAsync(registerRequest.UserName);
            if (isExistUser != null)
                return BadRequest("Username already exists");

            User newUser = new User()
            {
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerRequest.Password);
            if (!createUserResult.Succeeded)
            {
                var errorString = "User creation failed because:";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return BadRequest(errorString);
            }
            //Add a default User to all users
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            return Ok("User Created");
        }
        
        // Route -> Login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login ([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);
            if (user is null)
                return Unauthorized("Invalid Credentials");

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user,loginRequest.Password);
            if (!isPasswordCorrect)
                return Unauthorized("Invalid Credentials");

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName ?? throw new InvalidOperationException()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var token = GenerateNewJsonWebToken(authClaims);
            return Ok(token);
        }

        private string GenerateNewJsonWebToken(List<Claim> authClaims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ?? throw new InvalidOperationException()));
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }
        
        // Route -> Make User -> ADMIN
        [HttpPost]
        [Route("make_user_admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatePermissionRequest updatePermissionRequest)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionRequest.UserName);
            if (user is null)
                return BadRequest("Username does not exist");

            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);
            return Ok("User is now an Admin");
        }
        
        
        
        // Route -> Make User -> Owner
        [HttpPost]
        [Route("make_user_owner")]
        public async Task<IActionResult> MakeOwner([FromBody] UpdatePermissionRequest updatePermissionRequest)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionRequest.UserName);
            if (user is null)
                return BadRequest("Username does not exist");

            await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);
            return Ok("User is now an Owner");
        }
        
        
        // Route -> Remove Owner Role
        [HttpPost]
        [Route("remove_owner_role")]
        public async Task<IActionResult> RemoveOwnerRole([FromBody] UpdatePermissionRequest updatePermissionRequest)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionRequest.UserName);
            if (user is null)
                return BadRequest("Username does not exist");

            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.OWNER);
            return Ok("User is no longer an Owner");
        }
        
        
        // Route -> Remove Admin Role
        [HttpPost]
        [Route("remove_admin_role")]
        public async Task<IActionResult> RemoveAdminRole([FromBody] UpdatePermissionRequest updatePermissionRequest)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionRequest.UserName);
            if (user is null)
                return BadRequest("Username does not exist");

            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.ADMIN);
            return Ok("User is no longer an Admin");
        }
    }
}