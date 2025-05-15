using MathTutor.Core.Models;

namespace MathTutor.Application.Interfaces;

public interface IUserService
{
    Task<UserModel?> GetUserByIdAsync(string id);
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserModel>> GetAllUsersAsync();
    Task<UserModel?> UpdateUserAsync(UserModel userModel);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<bool> DeleteUserAsync(string id);
} 