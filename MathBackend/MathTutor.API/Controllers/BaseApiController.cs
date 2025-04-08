using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MathTutor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Gets the ID of the current user from the bearer token
    /// </summary>
    /// <returns>The user ID as a string, or null if not authenticated</returns>
    protected string? GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
    
    /// <summary>
    /// Handles a result from an operation and returns the appropriate HTTP response
    /// </summary>
    /// <typeparam name="T">The type of data in the result</typeparam>
    /// <param name="result">The result to handle</param>
    /// <returns>An appropriate ActionResult based on the result status</returns>
    protected IActionResult HandleResult<T>(T? result)
    {
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }
    
    /// <summary>
    /// Handles a result from an operation with more detailed status info
    /// </summary>
    /// <typeparam name="T">The type of data in the result</typeparam>
    /// <param name="result">The result to handle</param>
    /// <returns>An appropriate ActionResult based on the result status</returns>
    protected IActionResult HandleResult<T>(Result<T>? result)
    {
        if (result == null)
            return NotFound();
            
        if (result.IsSuccess && result.Value != null)
            return Ok(result.Value);
            
        if (result.IsSuccess && result.Value == null)
            return NotFound();
            
        return BadRequest(result.Error);
    }
    
    /// <summary>
    /// Represents the result of an operation with success status and optional data or error
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public string Error { get; set; } = string.Empty;
        
        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public static Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Error = error };
    }
} 