using Microsoft.AspNetCore.Mvc;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System;

namespace MathTutor.API.Controllers
{
    // Add this new DTO for direct evaluation
    public class DirectEvaluationRequestDto
    {
        public string Problem { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class MathProblemController : ControllerBase
    {
        private readonly IMathProblemService _mathProblemService;
        private readonly IAIservice _aiService;

        public MathProblemController(IMathProblemService mathProblemService, IAIservice aiService)
        {
            _mathProblemService = mathProblemService;
            _aiService = aiService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetAllProblems()
        {
            var problems = await _mathProblemService.GetAllProblemsAsync();
            return Ok(problems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MathProblemDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProblemById(int id)
        {
            var problem = await _mathProblemService.GetProblemByIdAsync(id);
            
            if (problem == null)
            {
                return NotFound();
            }
            
            return Ok(problem);
        }

        [HttpGet("topic/{topicId}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetProblemsByTopic(int topicId)
        {
            var problems = await _mathProblemService.GetProblemsByTopicAsync(topicId);
            return Ok(problems);
        }

        [HttpGet("difficulty/{difficulty}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetProblemsByDifficulty(DifficultyLevel difficulty)
        {
            var problems = await _mathProblemService.GetProblemsByDifficultyAsync(difficulty);
            return Ok(problems);
        }

        [HttpGet("topic/{topicId}/difficulty/{difficulty}")]
        [ProducesResponseType(typeof(IEnumerable<MathProblemDto>), 200)]
        public async Task<IActionResult> GetProblemsByTopicAndDifficulty(int topicId, DifficultyLevel difficulty)
        {
            var problems = await _mathProblemService.GetProblemsByTopicAndDifficultyAsync(topicId, difficulty);
            return Ok(problems);
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
                return Ok(generatedProblem);
            }
            catch (System.Exception ex)
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
                return Ok(evaluationResult);
            }
            catch (System.InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
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
                EvaluateMathAnswerResponseDto evaluationResult;
                
                try 
                {
                    // Try to deserialize the response
                    evaluationResult = JsonSerializer.Deserialize<EvaluateMathAnswerResponseDto>(aiResponse);
                }
                catch (JsonException)
                {
                    // If direct deserialization fails, try to extract the JSON portion
                    var jsonStart = aiResponse.IndexOf('{');
                    var jsonEnd = aiResponse.LastIndexOf('}');
                    
                    if (jsonStart >= 0 && jsonEnd > jsonStart)
                    {
                        var jsonPart = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                        evaluationResult = JsonSerializer.Deserialize<EvaluateMathAnswerResponseDto>(jsonPart);
                    }
                    else
                    {
                        return BadRequest("Failed to evaluate the answer due to invalid response format");
                    }
                }
                
                if (evaluationResult == null)
                {
                    return BadRequest("Failed to evaluate the answer");
                }
                
                return Ok(evaluationResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 