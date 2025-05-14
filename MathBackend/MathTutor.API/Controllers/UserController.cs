using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using System.Security.Claims;
using MathTutor.API.Constants;
using System;

namespace MathTutor.API.Controllers;

public class UserController : BaseApiController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);
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
    /// <returns>The updated user profile</returns>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> Update([FromBody] UserModel model)
    {
        try
        {
            // Ensure user can only update their own profile unless they're an admin
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (userId != model.Id && !User.IsInRole("Admin"))
                return Forbid();

            // Validate input
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
                return BadRequest(UserControllerConstants.ErrorMessages.NamesRequired);

            var updatedUser = await _userService.UpdateUserAsync(model);
            if (updatedUser == null)
                return NotFound(UserControllerConstants.ErrorMessages.UserNotFoundOrUpdateFailed);

            return Ok(updatedUser);
        }
        catch (Exception)
        {
            return StatusCode(500, UserControllerConstants.ErrorMessages.UpdateError);
        }
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
            return BadRequest(UserControllerConstants.ErrorMessages.PasswordsRequired);

        if (model.NewPassword.Length < 6)
            return BadRequest(UserControllerConstants.ErrorMessages.PasswordTooShort);

        var success = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
        if (!success)
            return BadRequest(UserControllerConstants.ErrorMessages.PasswordChangeFailed);

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
            return BadRequest(UserControllerConstants.ErrorMessages.DeleteFailed);
            
        return Ok();
    }
}