using MathTutor.Core.Entities;

namespace MathTutor.Application.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser> GetByIdAsync(string id);
    Task<ApplicationUser> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<bool> CreateAsync(ApplicationUser user, string password);
    Task<bool> UpdateAsync(ApplicationUser user);
    Task<bool> DeleteAsync(string id);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<bool> AddToRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
} 