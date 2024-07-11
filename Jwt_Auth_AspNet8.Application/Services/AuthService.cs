using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jwt_Auth_AspNet8.Application.Dto;
using Jwt_Auth_AspNet8.Application.Interfaces;
using Jwt_Auth_AspNet8.Application.Model;
using Jwt_Auth_AspNet8.Application.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Jwt_Auth_AspNet8.Application.Services;

public class AuthService : IAuthService
{
    
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthServiceResponseDto> SeedRolesAsync()
    {
        bool ownerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
        bool userRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
        bool adminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);

        if (adminRoleExists && userRoleExists && ownerRoleExists)
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Role Seeding Already Done"
            };
            
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "Role Seeding Done Successfully"
        };
    }

    public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var isExistUser = await _userManager.FindByEmailAsync(registerDto.UserName);
        if (isExistUser != null)
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Username already exists"
            };
        
        User newUser = new User()
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);
        if (!createUserResult.Succeeded)
        {
            var errorString = "User creation failed because:";
            foreach (var error in createUserResult.Errors)
            {
                errorString += " # " + error.Description;
            }
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = errorString
            };
        }
        //Add a default User to all users
        await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "User Created Successfully"
        };
    }
    public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);
        if (user is null)
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Invalid Credentials"
            };

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user,loginDto.Password);
        if (!isPasswordCorrect)
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Invalid Credentials"
            };


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
        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = token
        };
    }
    public async Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);
        if (user is null)
            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "Username does not exist"
            };
        await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);
        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "User is now an Admin"
        };
    }
    public async Task<AuthServiceResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);
        if (user is null)
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Username does not exist"
            };

        await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);
        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "User is now an Owner"
        };
    }
    public async Task<AuthServiceResponseDto> RemoveAdminRoleAsync (UpdatePermissionDto updatePermissionDto)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);
        if (user is null)
            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "Username does not exist"
            };

        await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.ADMIN);
        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "User is no longer an Owner"
        };
    }
    public async Task<AuthServiceResponseDto> RemoveOwnerRoleAsync (UpdatePermissionDto updatePermissionDto)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);
        if (user is null)
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Username does not exist"
            };

        await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.OWNER);
        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "User is no longer an Owner"
        };
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
}