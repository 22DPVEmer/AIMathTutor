using AutoMapper;
using MathTutor.Application.Constants;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Models;
using MathTutor.Core.Models.Auth;

namespace MathTutor.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IJwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseModel> RegisterAsync(RegisterModel model)
    {
        try
        {
            // Check if user exists
            var userExists = await _userRepository.GetByEmailAsync(model.Email);
            if (userExists != null)
                return new AuthResponseModel
                {
                    Success = false,
                    Message = AuthServiceConstants.UserAlreadyExistsMessage,
                    Errors = new List<string> { AuthServiceConstants.EmailAlreadyRegisteredError }
                };

            // Create new user
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var succeeded = await _userRepository.CreateAsync(user, model.Password);

            if (!succeeded)
                return new AuthResponseModel
                {
                    Success = false,
                    Message = AuthServiceConstants.UserCreationFailedMessage,
                    Errors = new List<string> { AuthServiceConstants.FailedToCreateUserError }
                };

            // Ensure User role exists
            if (!await _roleRepository.RoleExistsAsync(AuthServiceConstants.DefaultUserRole))
                await _roleRepository.CreateRoleAsync(AuthServiceConstants.DefaultUserRole);

            // Add default user role
            // Refetch user to get the created Id
            user = await _userRepository.GetByEmailAsync(model.Email);
            await _userRepository.AddToRoleAsync(user, AuthServiceConstants.DefaultUserRole);

            return new AuthResponseModel
            {
                Success = true,
                Message = AuthServiceConstants.RegistrationSuccessMessage
            };
        }
        catch (Exception)
        {
            return new AuthResponseModel
            {
                Success = false,
                Message = AuthServiceConstants.RegistrationFailedMessage,
                Errors = new List<string> { AuthServiceConstants.RegistrationErrorMessage }
            };
        }
    }

    public async Task<AuthResponseModel> LoginAsync(LoginModel model)
    {
        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponseModel
                {
                    Success = false,
                    Message = AuthServiceConstants.InvalidLoginAttemptMessage,
                    Errors = new List<string> { AuthServiceConstants.InvalidCredentialsError }
                };

            // Check password
            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return new AuthResponseModel
                {
                    Success = false,
                    Message = AuthServiceConstants.InvalidLoginAttemptMessage,
                    Errors = new List<string> { AuthServiceConstants.InvalidCredentialsError }
                };

            // Get user roles
            var roles = await _userRepository.GetRolesAsync(user);

            // Generate JWT token
            var token = _jwtTokenService.GenerateJwtToken(user, roles);

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return new AuthResponseModel
            {
                Success = true,
                Token = token,
                Message = AuthServiceConstants.LoginSuccessMessage,
                User = _mapper.Map<UserModel>(user)
            };
        }
        catch (Exception)
        {
            return new AuthResponseModel
            {
                Success = false,
                Message = AuthServiceConstants.LoginFailedMessage,
                Errors = new List<string> { AuthServiceConstants.LoginErrorMessage }
            };
        }
    }

    public async Task<AuthResponseModel> RefreshTokenAsync(string token)
    {
        try
        {
            var (userId, roles) = _jwtTokenService.ValidateJwtToken(token);

            if (string.IsNullOrEmpty(userId))
                return new AuthResponseModel
                {
                    Success = false,
                    Message = AuthServiceConstants.InvalidTokenMessage,
                    Errors = new List<string> { AuthServiceConstants.TokenValidationFailedError }
                };

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new AuthResponseModel
                {
                    Success = false,
                    Message = AuthServiceConstants.UserNotFoundMessage,
                    Errors = new List<string> { AuthServiceConstants.UserNotFoundWithTokenError }
                };

            var newToken = _jwtTokenService.GenerateJwtToken(user, roles);

            return new AuthResponseModel
            {
                Success = true,
                Token = newToken,
                Message = AuthServiceConstants.TokenRefreshedMessage,
                User = _mapper.Map<UserModel>(user)
            };
        }
        catch (Exception)
        {
            return new AuthResponseModel
            {
                Success = false,
                Message = AuthServiceConstants.TokenRefreshFailedMessage,
                Errors = new List<string> { AuthServiceConstants.TokenRefreshErrorMessage }
            };
        }
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        // In a JWT setup, client-side logout is sufficient
        return true;
    }

    public async Task<UserModel> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        var roles = await _userRepository.GetRolesAsync(user);
        var userModel = _mapper.Map<UserModel>(user);
        userModel.Roles = roles.ToList();

        return userModel;
    }

    public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
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
}