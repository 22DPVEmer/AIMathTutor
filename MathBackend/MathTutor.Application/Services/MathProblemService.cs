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
                
                GeneratedMathProblemResponseDto generatedProblem;
                
                try 
                {
                    // Try to deserialize the response
                    generatedProblem = JsonSerializer.Deserialize<GeneratedMathProblemResponseDto>(aiResponse);
                }
                catch (JsonException)
                {
                    // If direct deserialization fails, try to extract the JSON portion
                    // Gemini sometimes includes additional text around the JSON
                    var jsonStart = aiResponse.IndexOf('{');
                    var jsonEnd = aiResponse.LastIndexOf('}');
                    
                    if (jsonStart >= 0 && jsonEnd > jsonStart)
                    {
                        var jsonPart = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                        generatedProblem = JsonSerializer.Deserialize<GeneratedMathProblemResponseDto>(jsonPart);
                    }
                    else
                    {
                        _logger.LogWarning("Could not extract valid JSON from AI response");
                        throw new InvalidOperationException("Failed to generate a valid math problem");
                    }
                }
                
                if (generatedProblem == null || string.IsNullOrWhiteSpace(generatedProblem.Statement))
                {
                    _logger.LogWarning("AI generated an invalid math problem response");
                    throw new InvalidOperationException("Failed to generate a valid math problem");
                }
                
                // If TopicId is provided, we can automatically store the generated problem in the database
                if (request.TopicId > 0)
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
                
                string aiResponse = await _aiService.EvaluateAnswerAsync(problem.Statement, request.UserAnswer);
                
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
                        _logger.LogWarning("Could not extract valid JSON from AI response");
                        throw new InvalidOperationException("Failed to evaluate the answer");
                    }
                }
                
                if (evaluationResult == null)
                {
                    _logger.LogWarning("AI generated an invalid evaluation response");
                    throw new InvalidOperationException("Failed to evaluate the answer");
                }
                
                return evaluationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating math answer");
                throw;
            }
        }

        private string MapDifficultyToString(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => "Easy",
                "hard" => "Hard",
                _ => "Medium"
            };
        }

        private DifficultyLevel MapStringToDifficulty(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => DifficultyLevel.Easy,
                "hard" => DifficultyLevel.Hard,
                _ => DifficultyLevel.Medium
            };
        }
    }
} 