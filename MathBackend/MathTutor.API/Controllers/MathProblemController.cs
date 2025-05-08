using Microsoft.AspNetCore.Mvc;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System;
using MathTutor.Core.Models;
using System.Security.Claims;
using AutoMapper;

namespace MathTutor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MathProblemController : BaseApiController
    {
        private readonly IMathProblemService _mathProblemService;
        private readonly IAIservice _aiService;

        public MathProblemController(
            IMathProblemService mathProblemService,
            IAIservice aiService)
        {
            _mathProblemService = mathProblemService ?? throw new ArgumentNullException(nameof(mathProblemService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MathProblemModel>), 200)]
        public async Task<IActionResult> GetAllProblems()
        {
            var problems = await _mathProblemService.GetAllProblemsAsync();
            return HandleResult(problems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MathProblemModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProblemById(int id)
        {
            var problem = await _mathProblemService.GetProblemByIdAsync(id);
            return HandleResult(problem);
        }

        [HttpGet("topic/{topicId}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemModel>), 200)]
        public async Task<IActionResult> GetProblemsByTopic(int topicId)
        {
            try
            {
                if (topicId <= 0)
                {
                    return BadRequest("Invalid topic ID");
                }

                var problems = await _mathProblemService.GetProblemsByTopicAsync(topicId);
                return Ok(problems ?? new List<MathProblemModel>());
            }
            catch (Exception ex)
            {
                // Log the exception but return an empty list
                Console.WriteLine($"Error fetching problems for topic {topicId}: {ex.Message}");
                return Ok(new List<MathProblemModel>());
            }
        }

        [HttpGet("difficulty/{difficulty}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemModel>), 200)]
        public async Task<IActionResult> GetProblemsByDifficulty(DifficultyLevel difficulty)
        {
            var problems = await _mathProblemService.GetProblemsByDifficultyAsync(difficulty);
            return HandleResult(problems);
        }

        [HttpGet("topic/{topicId}/difficulty/{difficulty}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemModel>), 200)]
        public async Task<IActionResult> GetProblemsByTopicAndDifficulty(int topicId, DifficultyLevel difficulty)
        {
            var problems = await _mathProblemService.GetProblemsByTopicAndDifficultyAsync(topicId, difficulty);
            return HandleResult(problems);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(MathProblemModel), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateProblem(CreateMathProblemDto problemDto)
        {
            var createdProblem = await _mathProblemService.CreateProblemAsync(problemDto);
            return CreatedAtAction(nameof(GetProblemById), new { id = createdProblem.Id }, createdProblem);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProblem(int id, UpdateMathProblemDto problemDto)
        {
            var result = await _mathProblemService.UpdateProblemAsync(id, problemDto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProblem(int id)
        {
            var result = await _mathProblemService.DeleteProblemAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("generate")]
        [ProducesResponseType(typeof(GeneratedMathProblemResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GenerateProblem(GenerateMathProblemRequestDto request)
        {
            try
            {
                var generatedProblem = await _mathProblemService.GenerateMathProblemAsync(request);
                return HandleResult(generatedProblem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("evaluate")]
        [ProducesResponseType(typeof(EvaluateMathAnswerResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> EvaluateAnswer(EvaluateMathAnswerRequestDto request)
        {
            try
            {
                var evaluationResult = await _mathProblemService.EvaluateAnswerAsync(request);
                return HandleResult(evaluationResult);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("evaluate-direct")]
        [ProducesResponseType(typeof(EvaluateMathAnswerResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EvaluateDirectly([FromBody] DirectEvaluationRequestDto request)
        {
            try
            {
                // Log the incoming request for debugging
                Console.WriteLine($"Received evaluation request: Problem={request?.Problem?.Length ?? 0} chars, " +
                                 $"UserAnswer={request?.UserAnswer?.Length ?? 0} chars");

                if (request == null)
                {
                    return BadRequest("Request body is required");
                }

                if (string.IsNullOrWhiteSpace(request.Problem) || string.IsNullOrWhiteSpace(request.UserAnswer))
                {
                    return BadRequest("Problem statement and user answer are required");
                }

                // Special case handling for quadratic equations
                if (_mathProblemService.IsQuadraticEquation(request.Problem))
                {
                    Console.WriteLine("Detected quadratic equation. Using AI evaluation for quadratic equation.");
                }

                string aiResponse = await _aiService.EvaluateAnswerAsync(request.Problem, request.UserAnswer);

                // Log the AI response for debugging
                Console.WriteLine($"AI response for evaluation: {aiResponse?.Length ?? 0} chars");

                if (string.IsNullOrEmpty(aiResponse))
                {
                    return BadRequest("Failed to get a valid response from the AI service");
                }

                return await ParseAiResponseAsync<EvaluateMathAnswerResponseDto>(aiResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EvaluateDirectly: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("get-guidance")]
        [ProducesResponseType(typeof(GuidanceResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetGuidance([FromBody] GuidanceRequestDto request)
        {
            try
            {
                // Log the incoming request for debugging
                Console.WriteLine($"Received guidance request: Problem={request?.Problem?.Length ?? 0} chars, " +
                                 $"Solution={request?.Solution?.Length ?? 0} chars, " +
                                 $"UserAnswer={request?.UserAnswer?.Length ?? 0} chars, " +
                                 $"Question={request?.Question?.Length ?? 0} chars");

                if (request == null)
                {
                    return BadRequest("Request body is required");
                }

                if (string.IsNullOrWhiteSpace(request.Problem) ||
                    string.IsNullOrWhiteSpace(request.Solution) ||
                    string.IsNullOrWhiteSpace(request.Question))
                {
                    return BadRequest("Problem statement, solution, and question are required");
                }

                string aiResponse = await _aiService.GetGuidanceAsync(
                    request.Problem,
                    request.Solution,
                    request.UserAnswer ?? string.Empty,
                    request.Question);

                // Log the AI response for debugging
                Console.WriteLine($"AI response for guidance: {aiResponse?.Length ?? 0} chars");

                if (string.IsNullOrEmpty(aiResponse))
                {
                    return BadRequest("Failed to get a valid guidance response from the AI service");
                }

                return await ParseAiResponseAsync<GuidanceResponseDto>(aiResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGuidance: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("save-attempt")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SaveProblemAttempt([FromBody] SaveProblemAttemptDto attemptDto)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                attemptDto.UserId = userId;
                var result = await _mathProblemService.SaveProblemAttemptAsync(attemptDto);

                if (result && attemptDto.TopicId.HasValue)
                {
                    // Get updated problems for this topic to reflect the new attempt
                    var problems = await _mathProblemService.GetProblemsByTopicAsync(attemptDto.TopicId.Value);

                    // Get user attempts for these problems
                    var attempts = await _mathProblemService.GetAttemptsByUserIdAsync(userId);

                    // Return both the result and the updated problems
                    return Ok(new {
                        success = result,
                        problems = problems,
                        attempts = attempts
                    });
                }

                return HandleResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("evaluate-and-save")]
        [Authorize]
        [ProducesResponseType(typeof(EvaluateAndSaveResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> EvaluateAndSave([FromBody] EvaluateAndSaveRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body is required");
                }

                if (string.IsNullOrWhiteSpace(request.Problem) || string.IsNullOrWhiteSpace(request.UserAnswer))
                {
                    return BadRequest("Problem statement and user answer are required");
                }

                // Get the user ID
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Use the service to evaluate and save the problem
                var result = await _mathProblemService.EvaluateAndSaveAsync(request, userId);

                // Map the result to the response DTO
                var response = new EvaluateAndSaveResponseDto
                {
                    Success = result.Success,
                    IsCorrect = result.IsCorrect,
                    Feedback = result.Feedback,
                    HasExistingCorrectAttempt = result.HasExistingCorrectAttempt,
                    Problems = result.Problems,
                    Attempts = result.Attempts
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}