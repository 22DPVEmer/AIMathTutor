using Microsoft.AspNetCore.Identity;

namespace MathTutor.Application.Interfaces;

public interface IRoleRepository
{
    Task<bool> RoleExistsAsync(string roleName);
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    Task<IdentityRole> GetRoleByIdAsync(string roleId);
    Task<IdentityRole> GetRoleByNameAsync(string roleName);
} 