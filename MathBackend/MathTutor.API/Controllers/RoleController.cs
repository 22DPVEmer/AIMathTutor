using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using MathTutor.API.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MathTutor.API.Controllers;

[Authorize(Roles = "Admin")]
public class RoleController : BaseApiController
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public RoleController(
        IRoleRepository roleRepository,
        IUserRepository userRepository)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
        return HandleResult(roles);
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
            return BadRequest(RoleControllerConstants.ErrorMessages.RoleNameEmpty);

        // Check if role already exists
        if (await _roleRepository.RoleExistsAsync(roleName))
            return BadRequest(string.Format(RoleControllerConstants.ErrorMessages.RoleAlreadyExists, roleName));

        var result = await _roleRepository.CreateRoleAsync(roleName);
        if (!result.Succeeded)
            return BadRequest(string.Format(RoleControllerConstants.ErrorMessages.FailedToCreateRole, 
                string.Join(", ", result.Errors.Select(e => e.Description))));

        return Ok(new { Success = true, Message = string.Format(RoleControllerConstants.SuccessMessages.RoleCreated, roleName) });
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
            return BadRequest(RoleControllerConstants.ErrorMessages.UserIdAndRoleRequired);

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(model.UserId);
        if (user == null)
            return NotFound(RoleControllerConstants.ErrorMessages.UserNotFound);

        // Check if role exists
        if (!await _roleRepository.RoleExistsAsync(model.RoleName))
            return NotFound(string.Format(RoleControllerConstants.ErrorMessages.RoleNotFound, model.RoleName));

        // Check if user is already in role
        var userRoles = await _userRepository.GetRolesAsync(user);
        if (userRoles.Contains(model.RoleName))
            return BadRequest(string.Format(RoleControllerConstants.ErrorMessages.UserAlreadyInRole, model.RoleName));

        // Add user to role
        var success = await _userRepository.AddToRoleAsync(user, model.RoleName);
        if (!success)
            return BadRequest(RoleControllerConstants.ErrorMessages.FailedToAddUserToRole);

        return Ok(new { Success = true, Message = string.Format(RoleControllerConstants.SuccessMessages.UserAddedToRole, model.RoleName) });
    }
}