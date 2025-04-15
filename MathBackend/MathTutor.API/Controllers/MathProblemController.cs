using Microsoft.AspNetCore.Mvc;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System;
using System.Security.Claims;

namespace MathTutor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MathProblemController : BaseApiController
    {
        private readonly IMathProblemService _mathProblemService;
        private readonly IAIservice _aiService;

        public MathProblemController(IMathProblemService mathProblemService, IAIservice aiService)
        {
            _mathProblemService = mathProblemService ?? throw new ArgumentNullException(nameof(mathProblemService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetAllProblems()
        {
            var problems = await _mathProblemService.GetAllProblemsAsync();
            return HandleResult(problems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MathProblemDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProblemById(int id)
        {
            var problem = await _mathProblemService.GetProblemByIdAsync(id);
            return HandleResult(problem);
        }

        [HttpGet("topic/{topicId}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetProblemsByTopic(int topicId)
        {
            var problems = await _mathProblemService.GetProblemsByTopicAsync(topicId);
            return HandleResult(problems);
        }

        [HttpGet("difficulty/{difficulty}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetProblemsByDifficulty(DifficultyLevel difficulty)
        {
            var problems = await _mathProblemService.GetProblemsByDifficultyAsync(difficulty);
            return HandleResult(problems);
        }

        [HttpGet("topic/{topicId}/difficulty/{difficulty}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetProblemsByTopicAndDifficulty(int topicId, DifficultyLevel difficulty)
        {
            var problems = await _mathProblemService.GetProblemsByTopicAndDifficultyAsync(topicId, difficulty);
            return HandleResult(problems);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(MathProblemDto), 201)]
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
        public async Task<IActionResult> EvaluateDirectly(DirectEvaluationRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Problem) || string.IsNullOrWhiteSpace(request.UserAnswer))
                {
                    return BadRequest("Problem statement and user answer are required");
                }
                
                string aiResponse = await _aiService.EvaluateAnswerAsync(request.Problem, request.UserAnswer);
                return await ParseAiResponseAsync<EvaluateMathAnswerResponseDto>(aiResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("save-attempt")]
        [Authorize]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SaveProblemAttempt(SaveProblemAttemptDto attemptDto)
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
                return HandleResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 