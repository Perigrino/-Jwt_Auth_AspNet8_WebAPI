using Jwt_Auth_AspNet8.Contracts.OtherObjects;
using Jwt_Auth_AspNet8.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jwt_Auth_AspNet8.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        
        // Route For Sedding My Roles to DB
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

            IdentityUser newUser = new IdentityUser()
            {
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

        
        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}