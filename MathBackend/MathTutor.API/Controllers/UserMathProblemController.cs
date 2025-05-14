using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MathTutor.Application.DTOs;
using MathTutor.Application.Services;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using MathTutor.Core.Enums;
using MathTutor.API.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace MathTutor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserMathProblemController : BaseApiController
    {
        private readonly IUserMathProblemService _userMathProblemService;
        private readonly IAIservice _aiService;

        public UserMathProblemController(
            IUserMathProblemService userMathProblemService,
            IAIservice aiService)
        {
            _userMathProblemService = userMathProblemService ?? throw new ArgumentNullException(nameof(userMathProblemService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
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
                    return Unauthorized(UserMathProblemControllerConstants.ErrorMessages.UserNotAuthenticated);
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
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(UserMathProblemControllerConstants.ErrorMessages.UserNotAuthenticated);
                }

                model.UserId = userId;

                var result = await _userMathProblemService.CreateUserMathProblemAsync(model);
                if (result == null)
                {
                    return BadRequest(UserMathProblemControllerConstants.ErrorMessages.FailedToCreateProblem);
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

                var userId = GetUserId();
                if (userId != problem.UserId && !User.IsInRole("Admin") && !User.IsInRole("Teacher"))
                {
                    return Forbid();
                }

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
                    return BadRequest(UserMathProblemControllerConstants.ErrorMessages.FailedToUpdateProblem);
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

                var userId = GetUserId();
                if (userId != problem.UserId && !User.IsInRole("Admin") && !User.IsInRole("Teacher"))
                {
                    return Forbid();
                }

                var result = await _userMathProblemService.DeleteUserMathProblemAsync(id);
                if (!result)
                {
                    return BadRequest(UserMathProblemControllerConstants.ErrorMessages.FailedToDeleteProblem);
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

                var userId = GetUserId();
                if (userId != problem.UserId)
                {
                    return Forbid();
                }

                return await EvaluateAndSaveAsync(problem, request.UserAnswer);
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
                    return Unauthorized(UserMathProblemControllerConstants.ErrorMessages.UserNotAuthenticated);
                }

                attemptDto.UserId = userId;

                var userMathProblem = new UserMathProblemModel
                {
                    UserId = userId,
                    TopicName = attemptDto.Topic,
                    Statement = attemptDto.Statement,
                    Solution = attemptDto.Solution,
                    Explanation = attemptDto.Explanation,
                    UserAnswer = attemptDto.UserAnswer,
                    IsCorrect = attemptDto.IsCorrect,
                    Difficulty = attemptDto.Difficulty,
                    TopicId = attemptDto.TopicId,
                    CreatedAt = DateTime.UtcNow
                };

                var savedProblem = await _userMathProblemService.CreateUserMathProblemAsync(userMathProblem);
                if (savedProblem == null)
                {
                    return BadRequest(UserMathProblemControllerConstants.ErrorMessages.FailedToCreateProblem);
                }

                return CreatedAtAction(nameof(GetUserMathProblemById), new { id = savedProblem.Id }, savedProblem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("publish/{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PublishUserMathProblem(int id, [FromQuery] string? name = null)
        {
            try
            {
                var userProblem = await _userMathProblemService.GetUserMathProblemByIdAsync(id);
                if (userProblem == null)
                {
                    return NotFound(string.Format(UserMathProblemControllerConstants.ErrorMessages.ProblemNotFound, id));
                }

                if (!userProblem.TopicId.HasValue || userProblem.TopicId.Value <= 0)
                {
                    return BadRequest(UserMathProblemControllerConstants.ErrorMessages.TopicRequired);
                }

                var mathProblemService = HttpContext.RequestServices.GetRequiredService<IMathProblemService>();
                var createProblemDto = new CreateMathProblemDto
                {
                    Name = !string.IsNullOrWhiteSpace(name) ? name : string.Format(UserMathProblemControllerConstants.DefaultValues.DefaultProblemName, userProblem.TopicName),
                    Statement = userProblem.Statement,
                    Solution = userProblem.Solution,
                    Explanation = userProblem.Explanation,
                    Difficulty = MapStringToDifficulty(userProblem.Difficulty),
                    TopicId = userProblem.TopicId.Value,
                    PointValue = userProblem.PointValue
                };

                var publishedProblem = await mathProblemService.CreateProblemAsync(createProblemDto);

                return Ok(new { success = true, message = UserMathProblemControllerConstants.SuccessMessages.PublishedSuccessfully, publishedProblemId = publishedProblem.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private static DifficultyLevel MapStringToDifficulty(string difficulty)
        {
            return difficulty?.ToLower() switch
            {
                "easy" => DifficultyLevel.Easy,
                "medium" => DifficultyLevel.Medium,
                "hard" => DifficultyLevel.Hard,
                _ => DifficultyLevel.Medium
            };
        }

        private async Task<IActionResult> EvaluateAndSaveAsync(UserMathProblemModel problem, string userAnswer)
        {
            try
            {
                // DIRECT COMPARISON: Compare user answer with the solution
                // Normalize both strings by trimming whitespace and converting to lowercase
                string normalizedUserAnswer = userAnswer?.Trim() ?? string.Empty;
                string normalizedSolution = problem.Solution?.Trim() ?? string.Empty;

                // Direct comparison - if user answer matches solution exactly, it's correct
                bool isCorrect = string.Equals(normalizedUserAnswer, normalizedSolution, StringComparison.OrdinalIgnoreCase);

                string feedback;
                if (isCorrect)
                {
                    feedback = UserMathProblemControllerConstants.FeedbackMessages.CorrectAnswer;
                }
                else
                {
                    // Only use AI for feedback if the answer is incorrect
                    try
                    {
                        string safeUserAnswer = userAnswer ?? string.Empty;
                        string aiResponse = await _aiService.EvaluateAnswerAsync(problem.Statement, safeUserAnswer);
                        var parsedResponse = await ParseAiResponseAsync<EvaluateMathAnswerResponseDto>(aiResponse);

                        if (parsedResponse is OkObjectResult okResult &&
                            okResult.Value is EvaluateMathAnswerResponseDto evaluationResult)
                        {
                            feedback = evaluationResult.Feedback;
                        }
                        else
                        {
                            feedback = UserMathProblemControllerConstants.FeedbackMessages.IncorrectAnswer;
                        }
                    }
                    catch
                    {
                        feedback = UserMathProblemControllerConstants.FeedbackMessages.IncorrectAnswer;
                    }
                }

                problem.UserAnswer = userAnswer ?? string.Empty;
                problem.IsCorrect = isCorrect;

                var result = await _userMathProblemService.UpdateUserMathProblemAsync(problem.Id, problem);
                if (!result)
                {
                    return BadRequest(UserMathProblemControllerConstants.ErrorMessages.FailedToUpdateWithNewAnswer);
                }

                var updatedProblem = await _userMathProblemService.GetUserMathProblemByIdAsync(problem.Id);

                return Ok(new {
                    feedback = feedback,
                    isCorrect = isCorrect,
                    problem = updatedProblem
                });
            }
            catch (Exception ex)
            {
                return BadRequest(string.Format(UserMathProblemControllerConstants.ErrorMessages.EvaluationError, ex.Message));
            }
        }
    }
}