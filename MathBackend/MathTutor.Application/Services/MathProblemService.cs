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
        private readonly IAIservice _aiService;
        private readonly IMapper _mapper;
        private readonly ILogger<MathProblemService> _logger;

        public MathProblemService(
            IMathProblemRepository mathProblemRepository,
            IAIservice aiService,
            IMapper mapper,
            ILogger<MathProblemService> logger)
        {
            _mathProblemRepository = mathProblemRepository ?? throw new ArgumentNullException(nameof(mathProblemRepository));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
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

                // Normalize both answers for comparison
                string normalizedUserAnswer = NormalizeAnswer(request.UserAnswer);
                string normalizedSolution = NormalizeAnswer(problem.Solution);

                bool isCorrect = string.Equals(normalizedUserAnswer, normalizedSolution, StringComparison.OrdinalIgnoreCase);
                
                string feedback = isCorrect 
                    ? "Correct! " + problem.Explanation
                    : $"Incorrect. The correct answer is: {problem.Solution}. Here's why: {problem.Explanation}";

                return new EvaluateMathAnswerResponseDto
                {
                    IsCorrect = isCorrect,
                    Feedback = feedback
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating answer");
                throw;
            }
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
            // Normalize input by trimming and converting to lowercase
            string normalizedDifficulty = difficulty?.Trim().ToLower() ?? "medium";
            
            return normalizedDifficulty switch
            {
                "easy" or "beginner" or "1" => "Easy",
                "medium" or "intermediate" or "2" => "Medium",
                "hard" or "difficult" or "advanced" or "3" => "Hard",
                "very hard" or "expert" or "4" => "Very Hard",
                _ => "Medium" // Default to Medium if unknown
            };
        }

        private DifficultyLevel MapStringToDifficulty(string difficulty)
        {
            // Normalize input by trimming and converting to lowercase
            string normalizedDifficulty = difficulty?.Trim().ToLower() ?? "medium";
            
            return normalizedDifficulty switch
            {
                "easy" or "beginner" or "1" => DifficultyLevel.Easy,
                "medium" or "intermediate" or "2" => DifficultyLevel.Medium,
                "hard" or "difficult" or "advanced" or "3" => DifficultyLevel.Hard,
                "very hard" or "expert" or "4" => DifficultyLevel.Hard, // Map Very Hard to Hard since we only have 3 levels
                _ => DifficultyLevel.Medium // Default to Medium if unknown
            };
        }
    }
} 