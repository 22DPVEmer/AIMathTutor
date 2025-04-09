using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace MathTutor.API.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get the current user's profile
    /// </summary>
    /// <returns>User profile data</returns>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile()
    {
        Console.WriteLine("GetProfile called");
        
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);
        _logger.LogInformation("User profile retrieved for user ID: {UserId}", userId);
        return HandleResult(user);
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    /// <param name="id">The user ID</param>
    /// <returns>User data</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return HandleResult(user);
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="model">The updated user data</param>
    /// <returns>Result of the update operation</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UserModel model)
    {
        // Ensure user can only update their own profile unless they're an admin
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (userId != model.Id && !User.IsInRole("Admin"))
            return Forbid();

        // Validate input
        if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
            return BadRequest("First name and last name are required");

        var success = await _userService.UpdateUserAsync(model);
        if (!success)
            return NotFound();

        return Ok();
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="model">Password change data</param>
    /// <returns>Result of the password change operation</returns>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        // Ensure user can only change their own password
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (userId != model.UserId)
            return Forbid();

        // Validate input
        if (string.IsNullOrWhiteSpace(model.CurrentPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
            return BadRequest("Current password and new password are required");

        if (model.NewPassword.Length < 6)
            return BadRequest("New password must be at least 6 characters long");

        var success = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
        if (!success)
            return BadRequest("Failed to change password. Please ensure your current password is correct.");

        return Ok();
    }

    /// <summary>
    /// Delete a user account
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>Result of the delete operation</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
            
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
            return BadRequest("Failed to delete user");
            
        return Ok();
    }
}