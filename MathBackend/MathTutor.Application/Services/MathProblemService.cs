using AutoMapper;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    public class MathProblemService : IMathProblemService
    {
        private readonly IMathProblemRepository _mathProblemRepository;
        private readonly IMathTopicRepository _mathTopicRepository;
        private readonly IMathProblemAttemptRepository _mathProblemAttemptRepository;
        private readonly IAIservice _aiService;
        private readonly MathKernelService _mathKernelService;
        private readonly IMapper _mapper;
        private readonly ILogger<MathProblemService> _logger;

        public MathProblemService(
            IMathProblemRepository mathProblemRepository,
            IMathTopicRepository mathTopicRepository,
            IMathProblemAttemptRepository mathProblemAttemptRepository,
            IAIservice aiService,
            MathKernelService mathKernelService,
            IMapper mapper,
            ILogger<MathProblemService> logger)
        {
            _mathProblemRepository = mathProblemRepository ?? throw new ArgumentNullException(nameof(mathProblemRepository));
            _mathTopicRepository = mathTopicRepository ?? throw new ArgumentNullException(nameof(mathTopicRepository));
            _mathProblemAttemptRepository = mathProblemAttemptRepository ?? throw new ArgumentNullException(nameof(mathProblemAttemptRepository));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _mathKernelService = mathKernelService ?? throw new ArgumentNullException(nameof(mathKernelService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MathProblemDto>> GetAllProblemsAsync()
        {
            var problems = await _mathProblemRepository.GetAllProblemsAsync();
            return _mapper.Map<IEnumerable<MathProblemDto>>(problems);
        }

        public async Task<MathProblemDto> GetProblemByIdAsync(int id)
        {
            var problem = await _mathProblemRepository.GetProblemByIdAsync(id);
            return _mapper.Map<MathProblemDto>(problem);
        }

        public async Task<IEnumerable<MathProblemDto>> GetProblemsByTopicAsync(int topicId)
        {
            var problems = await _mathProblemRepository.GetProblemsByTopicAsync(topicId);
            return _mapper.Map<IEnumerable<MathProblemDto>>(problems);
        }

        public async Task<IEnumerable<MathProblemDto>> GetProblemsByDifficultyAsync(DifficultyLevel difficulty)
        {
            var problems = await _mathProblemRepository.GetProblemsByDifficultyAsync(difficulty);
            return _mapper.Map<IEnumerable<MathProblemDto>>(problems);
        }

        public async Task<IEnumerable<MathProblemDto>> GetProblemsByTopicAndDifficultyAsync(int topicId, DifficultyLevel difficulty)
        {
            var problems = await _mathProblemRepository.GetProblemsByTopicAndDifficultyAsync(topicId, difficulty);
            return _mapper.Map<IEnumerable<MathProblemDto>>(problems);
        }

        public async Task<MathProblemDto> CreateProblemAsync(CreateMathProblemDto problemDto)
        {
            var problem = _mapper.Map<MathProblem>(problemDto);
            var createdProblem = await _mathProblemRepository.CreateProblemAsync(problem);
            return _mapper.Map<MathProblemDto>(createdProblem);
        }

        public async Task<bool> UpdateProblemAsync(int id, UpdateMathProblemDto problemDto)
        {
            var existingProblem = await _mathProblemRepository.GetProblemByIdAsync(id);
            
            if (existingProblem == null)
            {
                return false;
            }

            _mapper.Map(problemDto, existingProblem);
            return await _mathProblemRepository.UpdateProblemAsync(existingProblem);
        }

        public async Task<bool> DeleteProblemAsync(int id)
        {
            return await _mathProblemRepository.DeleteProblemAsync(id);
        }

        public async Task<GeneratedMathProblemResponseDto> GenerateMathProblemAsync(GenerateMathProblemRequestDto request)
        {
            try
            {
                string difficultyLevel = MapDifficultyToString(request.Difficulty);
                string aiResponse = await _aiService.GenerateMathProblemAsync(request.Topic, difficultyLevel);
                
                _logger.LogDebug("Raw AI Response: {Response}", aiResponse);
                
                // Define serializer options with more permissive settings
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                
                GeneratedMathProblemResponseDto generatedProblem = null;
                
                try 
                {
                    // Try to deserialize the response
                    generatedProblem = JsonSerializer.Deserialize<GeneratedMathProblemResponseDto>(aiResponse, jsonOptions);
                    _logger.LogDebug("Deserialized problem: {Problem}", JsonSerializer.Serialize(generatedProblem));
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Initial deserialization failed, attempting fallback parsing");
                    
                    // If direct deserialization fails, try to extract the JSON portion
                    // Gemini sometimes includes additional text around the JSON
                    var jsonStart = aiResponse.IndexOf('{');
                    var jsonEnd = aiResponse.LastIndexOf('}');
                    
                    if (jsonStart >= 0 && jsonEnd > jsonStart)
                    {
                        var jsonPart = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                        _logger.LogDebug("Extracted JSON part: {JsonPart}", jsonPart);
                        
                        try {
                            generatedProblem = JsonSerializer.Deserialize<GeneratedMathProblemResponseDto>(jsonPart, jsonOptions);
                        }
                        catch (JsonException innerEx) {
                            _logger.LogWarning(innerEx, "Fallback parsing also failed");
                            throw new InvalidOperationException("Failed to parse the generated math problem");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Could not extract valid JSON from AI response");
                        throw new InvalidOperationException("Failed to generate a valid math problem");
                    }
                }
                
                if (generatedProblem == null)
                {
                    _logger.LogWarning("Deserialized problem is null");
                    throw new InvalidOperationException("Failed to generate a valid math problem");
                }
                
                if (string.IsNullOrWhiteSpace(generatedProblem.Statement))
                {
                    _logger.LogWarning("AI generated a problem with no statement");
                    throw new InvalidOperationException("Generated problem is missing a statement");
                }
                
                // If TopicId is provided and SaveToDatabase is true, store the generated problem
                if (request.TopicId > 0 && request.SaveToDatabase)
                {
                    DifficultyLevel difficulty = MapStringToDifficulty(request.Difficulty);
                    
                    var problemToCreate = new CreateMathProblemDto
                    {
                        Statement = generatedProblem.Statement,
                        Solution = generatedProblem.Solution,
                        Explanation = generatedProblem.Explanation,
                        Difficulty = difficulty,
                        TopicId = request.TopicId
                    };
                    
                    await CreateProblemAsync(problemToCreate);
                    _logger.LogInformation("Generated problem saved to database with TopicId: {TopicId}", request.TopicId);
                }
                
                return generatedProblem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating math problem");
                throw;
            }
        }

        public async Task<EvaluateMathAnswerResponseDto> EvaluateAnswerAsync(EvaluateMathAnswerRequestDto request)
        {
            try
            {
                var problem = await _mathProblemRepository.GetProblemByIdAsync(request.ProblemId);
                
                if (problem == null)
                {
                    throw new InvalidOperationException($"Math problem with ID {request.ProblemId} not found");
                }

                // Check for non-answer responses
                string[] nonAnswerResponses = { "i don't know", "idk", "dont know", "don't know", "no idea", "not sure", "unsure", "maybe", "probably", "perhaps" };
                string normalizedUserAnswer = NormalizeAnswer(request.UserAnswer);
                
                if (nonAnswerResponses.Contains(normalizedUserAnswer))
                {
                    return new EvaluateMathAnswerResponseDto
                    {
                        IsCorrect = false,
                        Feedback = "Please provide a mathematical answer. If you're unsure, try to work through the problem step by step."
                    };
                }

                // Use MathKernelService to validate and check equivalence
                bool isValidExpression = await _mathKernelService.ValidateMathExpressionAsync(normalizedUserAnswer);
                if (!isValidExpression)
                {
                    return new EvaluateMathAnswerResponseDto
                    {
                        IsCorrect = false,
                        Feedback = "Your answer is not a valid mathematical expression. Please check your input and try again."
                    };
                }

                bool isEquivalent = await _mathKernelService.CheckExpressionEquivalenceAsync(normalizedUserAnswer, problem.Solution);
                
                string feedback = isEquivalent 
                    ? "Correct! " + problem.Explanation
                    : $"Incorrect. The correct answer is: {problem.Solution}. Here's why: {problem.Explanation}";

                return new EvaluateMathAnswerResponseDto
                {
                    IsCorrect = isEquivalent,
                    Feedback = feedback
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating answer");
                throw;
            }
        }

        private bool ContainsMathematicalContent(string answer)
        {
            // Check for mathematical symbols, numbers, or variables
            return answer.Any(c => char.IsDigit(c) || 
                                 c == '+' || c == '-' || c == '*' || c == '/' || 
                                 c == '^' || c == '√' || c == 'π' || c == 'x' || 
                                 c == 'y' || c == 'z' || c == '=');
        }

        private string NormalizeAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return string.Empty;

            // Remove whitespace, convert to lowercase, and handle common math formatting
            return answer.Trim()
                .ToLower()
                .Replace(" ", "")
                .Replace("=", "")
                .Replace("≈", "")
                .Replace("~", "")
                .Replace("pi", "π")
                .Replace("sqrt", "√");
        }

        private string MapDifficultyToString(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => "Easy",
                "medium" => "Medium",
                "hard" => "Hard",
                _ => "Medium"
            };
        }

        private DifficultyLevel MapStringToDifficulty(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => DifficultyLevel.Easy,
                "medium" => DifficultyLevel.Medium,
                "hard" => DifficultyLevel.Hard,
                _ => DifficultyLevel.Medium
            };
        }

        public async Task<bool> SaveProblemAttemptAsync(SaveProblemAttemptDto attemptDto)
        {
            try
            {
                _logger.LogInformation("Saving problem attempt for user {UserId}", attemptDto.UserId);
                
                // Create a MathProblem entity if it doesn't exist yet
                MathProblem problem = null;
                
                // If a topicId is provided, we'll save this as a reusable problem
                if (attemptDto.TopicId.HasValue && attemptDto.TopicId.Value > 0)
                {
                    // Check if topic exists
                    var topic = await _mathTopicRepository.GetTopicByIdAsync(attemptDto.TopicId.Value);
                    if (topic == null)
                    {
                        _logger.LogWarning("Topic with ID {TopicId} not found", attemptDto.TopicId.Value);
                        return false;
                    }
                    
                    // Save the problem
                    problem = new MathProblem
                    {
                        Statement = attemptDto.Statement,
                        Solution = attemptDto.Solution,
                        Explanation = attemptDto.Explanation,
                        Difficulty = MapStringToDifficulty(attemptDto.Difficulty),
                        TopicId = attemptDto.TopicId.Value
                    };
                    
                    problem = await _mathProblemRepository.CreateProblemAsync(problem);
                    _logger.LogInformation("Problem created with ID {ProblemId}", problem.Id);
                }
                
                // Save the attempt
                var attempt = new MathProblemAttempt
                {
                    UserId = attemptDto.UserId,
                    ProblemId = problem?.Id ?? 0,
                    UserAnswer = attemptDto.UserAnswer,
                    IsCorrect = attemptDto.IsCorrect,
                    AttemptedAt = DateTime.UtcNow
                };
                
                if (problem != null)
                {
                    // If we created a problem, associate the attempt with it
                    await _mathProblemAttemptRepository.CreateAttemptAsync(attempt);
                    _logger.LogInformation("Attempt saved with problem ID {ProblemId}", problem.Id);
                }
                else
                {
                    // For attempts without a saved problem, we still record the attempt but store the problem details in a different way
                    // This could be extended to save the problem statement and other details in a JSON field or separate table
                    await _mathProblemAttemptRepository.CreateAttemptWithoutProblemAsync(attempt, attemptDto.Statement);
                    _logger.LogInformation("Attempt saved without persistent problem");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving problem attempt");
                return false;
            }
        }
    }
} 