using MathTutor.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MathTutor.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(
        RoleManager<IdentityRole> roleManager,
        ILogger<RoleRepository> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        try
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role exists: {RoleName}", roleName);
            return false;
        }
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        try
        {
            return await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role: {RoleName}", roleName);
            return IdentityResult.Failed(new IdentityError { Description = "Failed to create role due to an exception" });
        }
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
    {
        try
        {
            return await _roleManager.Roles.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all roles");
            return Enumerable.Empty<IdentityRole>();
        }
    }

    public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
    {
        try
        {
            return await _roleManager.FindByIdAsync(roleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding role by ID: {RoleId}", roleId);
            return null;
        }
    }

    public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
    {
        try
        {
            return await _roleManager.FindByNameAsync(roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding role by name: {RoleName}", roleName);
            return null;
        }
    }
} 