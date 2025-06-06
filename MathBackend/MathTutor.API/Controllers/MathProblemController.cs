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
using MathTutor.API.Constants;

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
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.InvalidTopicId);
                }

                var problems = await _mathProblemService.GetProblemsByTopicAsync(topicId);
                return Ok(problems ?? new List<MathProblemModel>());
            }
            catch (Exception ex)
            {
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
        [Authorize(Roles = "Admin,Teacher")]
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

        [HttpGet("test-ai")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> TestAiConnection()
        {
            try
            {
                var testRequest = new GenerateMathProblemRequestDto
                {
                    Topic = "Basic Arithmetic",
                    Difficulty = "Easy",
                    TopicId = 1,
                    SaveToDatabase = false
                };

                var result = await _mathProblemService.GenerateMathProblemAsync(testRequest);

                return Ok(new {
                    success = true,
                    message = "AI connection working",
                    response = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Ok(new {
                    success = false,
                    message = "AI connection failed",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace,
                    timestamp = DateTime.UtcNow
                });
            }
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
                if (request == null)
                {
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.RequestBodyRequired);
                }

                if (string.IsNullOrWhiteSpace(request.Problem) || string.IsNullOrWhiteSpace(request.UserAnswer))
                {
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.MissingProblemOrAnswer);
                }

                // Special case handling for quadratic equations
                if (_mathProblemService.IsQuadraticEquation(request.Problem))
                {
                }

                string aiResponse = await _aiService.EvaluateAnswerAsync(request.Problem, request.UserAnswer);

                if (string.IsNullOrEmpty(aiResponse))
                {
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.FailedAiResponse);
                }

                return await ParseAiResponseAsync<EvaluateMathAnswerResponseDto>(aiResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(string.Format(MathProblemControllerConstants.ErrorMessages.ParseError, ex.Message));
            }
        }

        [HttpPost("get-guidance")]
        [ProducesResponseType(typeof(GuidanceResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetGuidance([FromBody] GuidanceRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Problem))
                {
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.InvalidGuidanceRequest);
                }

                string aiResponse = await _aiService.GetGuidanceAsync(
                    request.Problem, 
                    request.Solution ?? string.Empty, 
                    request.UserAnswer ?? string.Empty, 
                    request.Question ?? string.Empty);

                if (string.IsNullOrEmpty(aiResponse))
                {
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.FailedGuidanceResponse);
                }

                return await ParseAiResponseAsync<GuidanceResponseDto>(aiResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(string.Format(MathProblemControllerConstants.ErrorMessages.ParseError, ex.Message));
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
                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Override the user ID in the DTO
                attemptDto.UserId = userId;

                var result = await _mathProblemService.SaveProblemAttemptAsync(attemptDto);
                
                if (result)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.FailedToSaveAttempt);
                }
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
                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _mathProblemService.EvaluateAndSaveAsync(request, userId);
                
                if (result == null)
                {
                    return BadRequest(MathProblemControllerConstants.ErrorMessages.FailedToEvaluateAndSave);
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}