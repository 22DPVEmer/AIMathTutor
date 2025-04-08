using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MathTutor.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        UserManager<ApplicationUser> userManager,
        ILogger<UserRepository> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApplicationUser> GetByIdAsync(string id)
    {
        try
        {
            return await _userManager.FindByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID {UserId}", id);
            return null;
        }
    }

    public async Task<ApplicationUser> GetByEmailAsync(string email)
    {
        try
        {
            return await _userManager.FindByEmailAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email {Email}", email);
            return null;
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        try
        {
            return await _userManager.Users.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return Enumerable.Empty<ApplicationUser>();
        }
    }

    public async Task<bool> CreateAsync(ApplicationUser user, string password)
    {
        try
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return false;
        }
    }

    public async Task<bool> UpdateAsync(ApplicationUser user)
    {
        try
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return false;
        }
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        try
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking password for user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<bool> AddToRoleAsync(ApplicationUser user, string role)
    {
        try
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to role {Role}", user.Id, role);
            return false;
        }
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        try
        {
            return await _userManager.GetRolesAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", user.Id);
            return new List<string>();
        }
    }

    public async Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        try
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", user.Id);
            return false;
        }
    }
} 