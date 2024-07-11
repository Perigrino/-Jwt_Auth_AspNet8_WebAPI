using Jwt_Auth_AspNet8.Application.Dto;
using Jwt_Auth_AspNet8.Application.Model;

namespace Jwt_Auth_AspNet8.Application.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResponseDto> SeedRolesAsync();
    Task<AuthServiceResponseDto> RegisterAsync (RegisterDto registerDto);
    Task<AuthServiceResponseDto> LoginAsync (LoginDto loginDto);
    Task<AuthServiceResponseDto> MakeAdminAsync (UpdatePermissionDto updatePermissionDto);
    Task<AuthServiceResponseDto> MakeOwnerAsync (UpdatePermissionDto updatePermissionDto);
    Task<AuthServiceResponseDto> RemoveAdminRoleAsync (UpdatePermissionDto updatePermissionDto);
    Task<AuthServiceResponseDto> RemoveOwnerRoleAsync (UpdatePermissionDto updatePermissionDto);
}