using AutoMapper;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Models;
using MathTutor.Core.Models.Auth;
using Microsoft.Extensions.Logging;

namespace MathTutor.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _logger = logger;
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
                    Message = "User already exists",
                    Errors = new List<string> { "Email is already registered" }
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
                    Message = "User creation failed",
                    Errors = new List<string> { "Failed to create user" }
                };

            // Ensure User role exists
            if (!await _roleRepository.RoleExistsAsync("User"))
                await _roleRepository.CreateRoleAsync("User");

            // Add default user role
            // Refetch user to get the created Id
            user = await _userRepository.GetByEmailAsync(model.Email);
            await _userRepository.AddToRoleAsync(user, "User");

            return new AuthResponseModel
            {
                Success = true,
                Message = "User registered successfully. Please verify your email address."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return new AuthResponseModel
            {
                Success = false,
                Message = "Registration failed",
                Errors = new List<string> { "An error occurred during registration" }
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
                    Message = "Invalid login attempt",
                    Errors = new List<string> { "Invalid email or password" }
                };

            // Check password
            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return new AuthResponseModel
                {
                    Success = false,
                    Message = "Invalid login attempt",
                    Errors = new List<string> { "Invalid email or password" }
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
                Message = "Login successful",
                User = _mapper.Map<UserModel>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return new AuthResponseModel
            {
                Success = false,
                Message = "Login failed",
                Errors = new List<string> { "An error occurred during login" }
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
                    Message = "Invalid token",
                    Errors = new List<string> { "Token validation failed" }
                };

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new AuthResponseModel
                {
                    Success = false,
                    Message = "User not found",
                    Errors = new List<string> { "User associated with token not found" }
                };

            var newToken = _jwtTokenService.GenerateJwtToken(user, roles);

            return new AuthResponseModel
            {
                Success = true,
                Token = newToken,
                Message = "Token refreshed successfully",
                User = _mapper.Map<UserModel>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return new AuthResponseModel
            {
                Success = false,
                Message = "Token refresh failed",
                Errors = new List<string> { "An error occurred while refreshing the token" }
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