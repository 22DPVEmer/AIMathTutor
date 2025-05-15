using AutoMapper;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;

namespace MathTutor.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserModel?> GetUserByIdAsync(string id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            var roles = await _userRepository.GetRolesAsync(user);
            var userModel = _mapper.Map<UserModel>(user);
            userModel.Roles = roles.ToList();

            return userModel;
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            var roles = await _userRepository.GetRolesAsync(user);
            var userModel = _mapper.Map<UserModel>(user);
            userModel.Roles = roles.ToList();

            return userModel;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var userModels = new List<UserModel>();

            foreach (var user in users)
            {
                var roles = await _userRepository.GetRolesAsync(user);
                var userModel = _mapper.Map<UserModel>(user);
                userModel.Roles = roles.ToList();
                userModels.Add(userModel);
            }

            return userModels;
        }
        catch
        {
            return Enumerable.Empty<UserModel>();
        }
    }

    public async Task<UserModel> UpdateUserAsync(UserModel userModel)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userModel.Id);
            if (user == null)
                return null;

            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            
            // Only update email if it has changed
            if (user.Email != userModel.Email)
            {
                // Email change requires verification in production
                user.Email = userModel.Email;
                user.UserName = userModel.Email;
                user.IsVerified = false;
            }
            
            var updatedUser = await _userRepository.UpdateAsync(user);
            var roles = await _userRepository.GetRolesAsync(updatedUser);
            var result = _mapper.Map<UserModel>(updatedUser);
            result.Roles = roles.ToList();
            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            return await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        try
        {
            return await _userRepository.DeleteAsync(id);
        }
        catch
        {
            return false;
        }
    }
} 