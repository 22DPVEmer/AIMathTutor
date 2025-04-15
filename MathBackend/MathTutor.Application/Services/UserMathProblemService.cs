using AutoMapper;
using Microsoft.Extensions.Logging;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using MathTutor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    public class UserMathProblemService : IUserMathProblemService
    {
        private readonly IUserMathProblemRepository _userMathProblemRepository;
        private readonly IMathTopicRepository _mathTopicRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserMathProblemService> _logger;

        public UserMathProblemService(
            IUserMathProblemRepository userMathProblemRepository,
            IMathTopicRepository mathTopicRepository,
            IMapper mapper,
            ILogger<UserMathProblemService> logger)
        {
            _userMathProblemRepository = userMathProblemRepository ?? throw new ArgumentNullException(nameof(userMathProblemRepository));
            _mathTopicRepository = mathTopicRepository ?? throw new ArgumentNullException(nameof(mathTopicRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<UserMathProblemModel>> GetAllUserMathProblemsAsync()
        {
            try
            {
                var userMathProblems = await _userMathProblemRepository.GetAllUserMathProblemsAsync();
                return _mapper.Map<IEnumerable<UserMathProblemModel>>(userMathProblems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user math problems");
                return Enumerable.Empty<UserMathProblemModel>();
            }
        }

        public async Task<UserMathProblemModel> GetUserMathProblemByIdAsync(int id)
        {
            try
            {
                var userMathProblem = await _userMathProblemRepository.GetUserMathProblemByIdAsync(id);
                if (userMathProblem == null)
                {
                    return null;
                }
                return _mapper.Map<UserMathProblemModel>(userMathProblem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problem with ID {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByUserIdAsync(string userId)
        {
            try
            {
                var userMathProblems = await _userMathProblemRepository.GetUserMathProblemsByUserIdAsync(userId);
                return _mapper.Map<IEnumerable<UserMathProblemModel>>(userMathProblems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for user ID {UserId}", userId);
                return Enumerable.Empty<UserMathProblemModel>();
            }
        }

        public async Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByTopicIdAsync(int topicId)
        {
            try
            {
                var userMathProblems = await _userMathProblemRepository.GetUserMathProblemsByTopicIdAsync(topicId);
                return _mapper.Map<IEnumerable<UserMathProblemModel>>(userMathProblems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for topic ID {TopicId}", topicId);
                return Enumerable.Empty<UserMathProblemModel>();
            }
        }

        public async Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByTopicNameAsync(string topicName)
        {
            try
            {
                var userMathProblems = await _userMathProblemRepository.GetUserMathProblemsByTopicNameAsync(topicName);
                return _mapper.Map<IEnumerable<UserMathProblemModel>>(userMathProblems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for topic name {TopicName}", topicName);
                return Enumerable.Empty<UserMathProblemModel>();
            }
        }

        public async Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByDifficultyAsync(string difficulty)
        {
            try
            {
                var difficultyLevel = MapStringToDifficulty(difficulty);
                var userMathProblems = await _userMathProblemRepository.GetUserMathProblemsByDifficultyAsync(difficultyLevel);
                return _mapper.Map<IEnumerable<UserMathProblemModel>>(userMathProblems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for difficulty {Difficulty}", difficulty);
                return Enumerable.Empty<UserMathProblemModel>();
            }
        }

        public async Task<UserMathProblemModel> CreateUserMathProblemAsync(UserMathProblemModel model)
        {
            try
            {
                var userMathProblem = _mapper.Map<UserMathProblem>(model);
                userMathProblem.Difficulty = MapStringToDifficulty(model.Difficulty);
                userMathProblem.CreatedAt = DateTime.UtcNow;

                userMathProblem = await _userMathProblemRepository.CreateUserMathProblemAsync(userMathProblem);
                if (userMathProblem == null)
                {
                    return null;
                }

                return _mapper.Map<UserMathProblemModel>(userMathProblem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user math problem for user {UserId}", model.UserId);
                return null;
            }
        }

        public async Task<bool> UpdateUserMathProblemAsync(int id, UserMathProblemModel model)
        {
            try
            {
                var existingProblem = await _userMathProblemRepository.GetUserMathProblemByIdAsync(id);
                if (existingProblem == null)
                {
                    return false;
                }

                // Update properties
                existingProblem.Statement = model.Statement;
                existingProblem.Solution = model.Solution;
                existingProblem.Explanation = model.Explanation;
                existingProblem.TopicName = model.TopicName;
                existingProblem.Difficulty = MapStringToDifficulty(model.Difficulty);
                existingProblem.UserAnswer = model.UserAnswer;
                existingProblem.IsCorrect = model.IsCorrect;
                existingProblem.TopicId = model.TopicId;

                return await _userMathProblemRepository.UpdateUserMathProblemAsync(existingProblem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user math problem with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteUserMathProblemAsync(int id)
        {
            try
            {
                return await _userMathProblemRepository.DeleteUserMathProblemAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user math problem with ID {Id}", id);
                return false;
            }
        }

        public async Task<UserMathProblemModel> SaveGeneratedProblemAsync(SaveProblemAttemptDto problemAttemptDto)
        {
            try
            {
                _logger.LogInformation("Saving generated problem for user {UserId}", problemAttemptDto.UserId);

                var userMathProblem = new UserMathProblem
                {
                    Statement = problemAttemptDto.Statement,
                    Solution = problemAttemptDto.Solution,
                    Explanation = problemAttemptDto.Explanation,
                    UserAnswer = problemAttemptDto.UserAnswer,
                    IsCorrect = problemAttemptDto.IsCorrect,
                    TopicName = problemAttemptDto.Topic,
                    Difficulty = MapStringToDifficulty(problemAttemptDto.Difficulty),
                    UserId = problemAttemptDto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    TopicId = problemAttemptDto.TopicId
                };

                userMathProblem = await _userMathProblemRepository.CreateUserMathProblemAsync(userMathProblem);
                if (userMathProblem == null)
                {
                    _logger.LogWarning("Failed to save user math problem for user {UserId}", problemAttemptDto.UserId);
                    return null;
                }

                _logger.LogInformation("User math problem saved with ID {Id}", userMathProblem.Id);
                return _mapper.Map<UserMathProblemModel>(userMathProblem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving generated problem for user {UserId}", problemAttemptDto.UserId);
                return null;
            }
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
    }
} 