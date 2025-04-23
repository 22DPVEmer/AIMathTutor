using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace MathTutor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserMathProblemController : BaseApiController
    {
        private readonly IUserMathProblemService _userMathProblemService;
        private readonly IMathProblemService _mathProblemService;
        private readonly IAIservice _aiService;
        private readonly IMapper _mapper;

        public UserMathProblemController(
            IUserMathProblemService userMathProblemService,
            IMathProblemService mathProblemService,
            IAIservice aiService,
            IMapper mapper)
        {
            _userMathProblemService = userMathProblemService ?? throw new ArgumentNullException(nameof(userMathProblemService));
            _mathProblemService = mathProblemService ?? throw new ArgumentNullException(nameof(mathProblemService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserMathProblemModel>), 200)]
        public async Task<IActionResult> GetAllUserMathProblems()
        {
            var problems = await _userMathProblemService.GetAllUserMathProblemsAsync();
            return HandleResult(problems);
        }

        [HttpGet("my-problems")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<UserMathProblemModel>), 200)]
        public async Task<IActionResult> GetCurrentUserProblems()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }
                
                var problems = await _userMathProblemService.GetUserMathProblemsByUserIdAsync(userId);
                return HandleResult(problems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserMathProblemModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserMathProblemById(int id)
        {
            var problem = await _userMathProblemService.GetUserMathProblemByIdAsync(id);
            return HandleResult(problem);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<UserMathProblemModel>), 200)]
        public async Task<IActionResult> GetUserMathProblemsByUserId(string userId)
        {
            var problems = await _userMathProblemService.GetUserMathProblemsByUserIdAsync(userId);
            return HandleResult(problems);
        }

        [HttpGet("topic/{topicId}")]
        [ProducesResponseType(typeof(IEnumerable<UserMathProblemModel>), 200)]
        public async Task<IActionResult> GetUserMathProblemsByTopicId(int topicId)
        {
            var problems = await _userMathProblemService.GetUserMathProblemsByTopicIdAsync(topicId);
            return HandleResult(problems);
        }

        [HttpGet("topic-name/{topicName}")]
        [ProducesResponseType(typeof(IEnumerable<UserMathProblemModel>), 200)]
        public async Task<IActionResult> GetUserMathProblemsByTopicName(string topicName)
        {
            var problems = await _userMathProblemService.GetUserMathProblemsByTopicNameAsync(topicName);
            return HandleResult(problems);
        }

        [HttpGet("difficulty/{difficulty}")]
        [ProducesResponseType(typeof(IEnumerable<UserMathProblemModel>), 200)]
        public async Task<IActionResult> GetUserMathProblemsByDifficulty(string difficulty)
        {
            var problems = await _userMathProblemService.GetUserMathProblemsByDifficultyAsync(difficulty);
            return HandleResult(problems);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(UserMathProblemModel), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUserMathProblem([FromBody] UserMathProblemModel model)
        {
            try
            {
                // Get the current user ID from the claims
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }
                
                // Set the user ID in the model
                model.UserId = userId;
                
                var result = await _userMathProblemService.CreateUserMathProblemAsync(model);
                if (result == null)
                {
                    return BadRequest("Failed to create user math problem");
                }
                
                return CreatedAtAction(nameof(GetUserMathProblemById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserMathProblem(int id, [FromBody] UserMathProblemModel model)
        {
            try
            {
                var problem = await _userMathProblemService.GetUserMathProblemByIdAsync(id);
                if (problem == null)
                {
                    return NotFound();
                }
                
                // Check if the current user is the owner of the problem
                var userId = GetUserId();
                if (userId != problem.UserId && !User.IsInRole("Admin") && !User.IsInRole("Teacher"))
                {
                    return Forbid();
                }
                
                // Ensure the ID is set correctly
                model.Id = id;
                
                // If topicId has changed, update the topicName as well
                if (model.TopicId.HasValue && model.TopicId != problem.TopicId)
                {
                    var mathTopicService = HttpContext.RequestServices.GetRequiredService<IMathTopicService>();
                    var topic = await mathTopicService.GetTopicByIdAsync(model.TopicId.Value);
                    if (topic != null)
                    {
                        model.TopicName = topic.Name;
                    }
                }
                
                var result = await _userMathProblemService.UpdateUserMathProblemAsync(id, model);
                if (!result)
                {
                    return BadRequest("Failed to update user math problem");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUserMathProblem(int id)
        {
            try
            {
                var problem = await _userMathProblemService.GetUserMathProblemByIdAsync(id);
                if (problem == null)
                {
                    return NotFound();
                }
                
                // Check if the current user is the owner of the problem
                var userId = GetUserId();
                if (userId != problem.UserId && !User.IsInRole("Admin") && !User.IsInRole("Teacher"))
                {
                    return Forbid();
                }
                
                var result = await _userMathProblemService.DeleteUserMathProblemAsync(id);
                if (!result)
                {
                    return BadRequest("Failed to delete user math problem");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("retry/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserMathProblemModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RetryProblem(int id, [FromBody] RetryProblemRequestDto request)
        {
            try
            {
                var problem = await _userMathProblemService.GetUserMathProblemByIdAsync(id);
                if (problem == null)
                {
                    return NotFound();
                }
                
                // Check if the current user is the owner of the problem
                var userId = GetUserId();
                if (userId != problem.UserId)
                {
                    return Forbid();
                }
                
                return await EvaluateAndSaveAttemptAsync(problem, request.UserAnswer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("save-generated")]
        [Authorize]
        [ProducesResponseType(typeof(UserMathProblemModel), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SaveGeneratedProblem([FromBody] SaveProblemAttemptDto attemptDto)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }
                
                attemptDto.UserId = userId;
                
                // Create the UserMathProblem
                var userMathProblem = new UserMathProblemModel
                {
                    UserId = userId,
                    TopicName = attemptDto.Topic,
                    Statement = attemptDto.Statement,
                    Difficulty = attemptDto.Difficulty
                };
                
                var savedProblem = await _userMathProblemService.CreateUserMathProblemAsync(userMathProblem);
                if (savedProblem == null)
                {
                    return BadRequest("Failed to create user math problem");
                }
                
                return CreatedAtAction(nameof(GetUserMathProblemById), new { id = savedProblem.Id }, savedProblem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        private async Task<IActionResult> EvaluateAndSaveAttemptAsync(UserMathProblemModel problem, string userAnswer)
        {
            try
            {
                // Evaluate the new answer using the AI service
                string aiResponse = await _aiService.EvaluateAnswerAsync(problem.Statement, userAnswer);
                
                // Parse the AI response
                var parsedResponse = await ParseAiResponseAsync<EvaluateMathAnswerResponseDto>(aiResponse);
                
                // If the response is not successful, return it directly
                if (parsedResponse is not OkObjectResult okResult || 
                    okResult.Value is not EvaluateMathAnswerResponseDto evaluationResult)
                {
                    return parsedResponse;
                }
                
                // Save the attempt
                var attemptDto = new SaveProblemAttemptDto
                {
                    UserId = GetUserId(),
                    Statement = problem.Statement,
                    UserAnswer = userAnswer,
                    IsCorrect = evaluationResult.IsCorrect,
                    TopicId = problem.TopicId,
                    Topic = problem.TopicName,
                    Difficulty = problem.Difficulty
                };
                
                var saved = await _mathProblemService.SaveProblemAttemptAsync(attemptDto);
                if (!saved)
                {
                    return BadRequest("Failed to save the problem attempt");
                }
                
                return Ok(evaluationResult);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error evaluating answer: {ex.Message}");
            }
        }
    }
} 