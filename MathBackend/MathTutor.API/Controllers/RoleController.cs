using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace MathTutor.API.Controllers;

[Authorize(Roles = "Admin")]
public class RoleController : BaseApiController
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RoleController> _logger;

    public RoleController(
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        ILogger<RoleController> logger)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    /// <returns>List of all roles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IdentityRole>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleRepository.GetAllRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="roleName">The name of the role to create</param>
    /// <returns>Result of the create operation</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("Role name cannot be empty");

        // Check if role already exists
        if (await _roleRepository.RoleExistsAsync(roleName))
            return BadRequest($"Role '{roleName}' already exists");

        var result = await _roleRepository.CreateRoleAsync(roleName);
        if (!result.Succeeded)
            return BadRequest($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return Ok();
    }

    /// <summary>
    /// Add a user to a role
    /// </summary>
    /// <param name="model">Model containing user ID and role name</param>
    /// <returns>Result of the operation</returns>
    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserToRole([FromBody] UserRoleModel model)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.RoleName))
            return BadRequest("User ID and role name are required");

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(model.UserId);
        if (user == null)
            return NotFound("User not found");

        // Check if role exists
        if (!await _roleRepository.RoleExistsAsync(model.RoleName))
            return NotFound($"Role '{model.RoleName}' not found");

        // Check if user is already in role
        var userRoles = await _userRepository.GetRolesAsync(user);
        if (userRoles.Contains(model.RoleName))
            return BadRequest($"User is already in role '{model.RoleName}'");

        // Add user to role
        var success = await _userRepository.AddToRoleAsync(user, model.RoleName);
        if (!success)
            return BadRequest("Failed to add user to role");

        return Ok();
    }
} 