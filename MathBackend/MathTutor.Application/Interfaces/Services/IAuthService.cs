using MathTutor.Core.Models;
using MathTutor.Core.Models.Auth;

namespace MathTutor.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseModel> RegisterAsync(RegisterModel model);
    Task<AuthResponseModel> LoginAsync(LoginModel model);
    Task<AuthResponseModel> RefreshTokenAsync(string token);
    Task<bool> LogoutAsync(string userId);
    Task<UserModel> GetUserByIdAsync(string id);
    Task<IEnumerable<UserModel>> GetAllUsersAsync();
} 