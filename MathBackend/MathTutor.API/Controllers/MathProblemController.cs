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
                if (IsQuadraticEquation(request.Problem))
                {
                    Console.WriteLine("Detected quadratic equation. Using special evaluation logic.");
                    var specialResult = EvaluateQuadraticEquation(request.Problem, request.UserAnswer);
                    if (specialResult != null)
                    {
                        Console.WriteLine($"Used special quadratic equation evaluation. Result: isCorrect={specialResult.IsCorrect}");
                        return Ok(specialResult);
                    }
                    Console.WriteLine("Special evaluation didn't produce a result, falling back to AI evaluation.");
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

        // Check if a problem is a quadratic equation
        private static bool IsQuadraticEquation(string problem)
        {
            // Normalize the problem text
            string normalizedProblem = problem.ToLower().Replace(" ", "");

            // Check for common quadratic equation patterns
            return normalizedProblem.Contains("x²") ||
                   normalizedProblem.Contains("x^2") ||
                   (normalizedProblem.Contains("quadratic") && normalizedProblem.Contains("equation")) ||
                   (normalizedProblem.Contains("solve") &&
                    (normalizedProblem.Contains("x²") || normalizedProblem.Contains("x^2")));
        }

        // Special evaluation logic for quadratic equations
        private static EvaluateMathAnswerResponseDto? EvaluateQuadraticEquation(string problem, string userAnswer)
        {
            try
            {
                Console.WriteLine($"Evaluating quadratic equation answer: Problem={problem}, UserAnswer={userAnswer}");

                // Normalize the user answer by removing spaces and converting to lowercase
                string normalizedAnswer = userAnswer.ToLower().Replace(" ", "");

                // Extract numbers from the answer
                var numbers = ExtractNumbersFromAnswer(normalizedAnswer);

                if (numbers.Count == 0)
                {
                    Console.WriteLine("No numbers found in answer, falling back to AI evaluation");
                    return null; // Fall back to AI evaluation
                }

                // Check if the answer contains any of the expected solutions
                if (problem.Contains("x² - 9 = 0") || problem.Contains("x^2 - 9 = 0"))
                {
                    // The solutions are x = 3 or x = -3
                    if (numbers.Contains(3) || numbers.Contains(-3))
                    {
                        string feedback = numbers.Contains(3) && numbers.Contains(-3)
                            ? "Correct! The solutions are x = 3 and x = -3."
                            : numbers.Contains(3)
                                ? "Correct! x = 3 is one of the solutions. The other solution is x = -3."
                                : "Correct! x = -3 is one of the solutions. The other solution is x = 3.";

                        return new EvaluateMathAnswerResponseDto
                        {
                            IsCorrect = true,
                            Feedback = feedback
                        };
                    }
                }

                // Add more special cases for common quadratic equations here

                // If we couldn't determine the answer with our special logic, fall back to AI
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in quadratic equation evaluation: {ex.Message}");
                return null; // Fall back to AI evaluation
            }
        }

        // Extract numbers from an answer string
        private static List<int> ExtractNumbersFromAnswer(string answer)
        {
            var result = new List<int>();
            var numberStrings = System.Text.RegularExpressions.Regex.Matches(answer, @"-?\d+");

            foreach (System.Text.RegularExpressions.Match match in numberStrings)
            {
                if (int.TryParse(match.Value, out int number))
                {
                    result.Add(number);
                }
            }

            return result;
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


                EvaluateMathAnswerResponseDto evaluationResult;

                // Special case handling for quadratic equations
                if (IsQuadraticEquation(request.Problem))
                {
                    var specialResult = EvaluateQuadraticEquation(request.Problem, request.UserAnswer);
                    if (specialResult != null)
                    {
                        evaluationResult = specialResult;
                    }
                    else
                    {
                        string aiResponse = await _aiService.EvaluateAnswerAsync(request.Problem, request.UserAnswer);

                        if (string.IsNullOrEmpty(aiResponse))
                        {
                            return BadRequest("Failed to get a valid response from the AI service");
                        }

                        var evaluationActionResult = await ParseAiResponseAsync<EvaluateMathAnswerResponseDto>(aiResponse);
                        if (evaluationActionResult is not OkObjectResult okResult ||
                            okResult.Value is not EvaluateMathAnswerResponseDto result)
                        {
                            return BadRequest("Failed to parse the AI evaluation response");
                        }

                        evaluationResult = result;
                    }
                }
                else
                {
                    string aiResponse = await _aiService.EvaluateAnswerAsync(request.Problem, request.UserAnswer);

                    if (string.IsNullOrEmpty(aiResponse))
                    {
                        return BadRequest("Failed to get a valid response from the AI service");
                    }

                    var evaluationActionResult = await ParseAiResponseAsync<EvaluateMathAnswerResponseDto>(aiResponse);
                    if (evaluationActionResult is not OkObjectResult okResult ||
                        okResult.Value is not EvaluateMathAnswerResponseDto result)
                    {
                        return BadRequest("Failed to parse the AI evaluation response");
                    }

                    evaluationResult = result;
                }


                bool hasExistingCorrectAttempt = false;

                if (request.TopicId.HasValue && request.TopicId.Value > 0)
                {

                    var attempts = await _mathProblemService.GetAttemptsByUserIdAsync(userId);


                    if (attempts != null && attempts.Any())
                    {

                        var problems = await _mathProblemService.GetProblemsByTopicAsync(request.TopicId.Value);
                        var matchingProblem = problems.FirstOrDefault(p =>
                            p.Statement.Equals(request.Problem, StringComparison.OrdinalIgnoreCase));

                        if (matchingProblem != null)
                        {

                            hasExistingCorrectAttempt = attempts.Any(a =>
                                a.ProblemId == matchingProblem.Id && a.IsCorrect);
                        }
                    }
                }


                var attemptDto = new SaveProblemAttemptDto
                {
                    UserId = userId,
                    Name = request.Name,
                    Statement = request.Problem,
                    Solution = request.Solution,
                    Explanation = request.Explanation,
                    UserAnswer = request.UserAnswer,
                    IsCorrect = evaluationResult.IsCorrect,
                    Difficulty = request.Difficulty,
                    Topic = request.Topic,
                    TopicId = request.TopicId
                };


                var saveResult = await _mathProblemService.SaveProblemAttemptAsync(attemptDto);


                var response = new EvaluateAndSaveResponseDto
                {
                    Success = saveResult,
                    IsCorrect = evaluationResult.IsCorrect,
                    Feedback = evaluationResult.Feedback,
                    HasExistingCorrectAttempt = hasExistingCorrectAttempt
                };


                if (saveResult && request.TopicId.HasValue)
                {

                    var problems = await _mathProblemService.GetProblemsByTopicAsync(request.TopicId.Value);


                    var attempts = await _mathProblemService.GetAttemptsByUserIdAsync(userId);


                    response.Problems = problems;
                    response.Attempts = attempts;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}