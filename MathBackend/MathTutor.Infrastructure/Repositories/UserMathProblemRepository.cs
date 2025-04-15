using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using MathTutor.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathTutor.Infrastructure.Repositories
{
    public class UserMathProblemRepository : IUserMathProblemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserMathProblemRepository> _logger;

        public UserMathProblemRepository(ApplicationDbContext context, ILogger<UserMathProblemRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<UserMathProblem>> GetAllUserMathProblemsAsync()
        {
            try
            {
                return await _context.UserMathProblems
                    .Include(p => p.User)
                    .Include(p => p.Topic)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user math problems");
                return Enumerable.Empty<UserMathProblem>();
            }
        }

        public async Task<UserMathProblem> GetUserMathProblemByIdAsync(int id)
        {
            try
            {
                return await _context.UserMathProblems
                    .Include(p => p.User)
                    .Include(p => p.Topic)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problem with ID {ProblemId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByUserIdAsync(string userId)
        {
            try
            {
                return await _context.UserMathProblems
                    .Where(p => p.UserId == userId)
                    .Include(p => p.User)
                    .Include(p => p.Topic)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for user ID {UserId}", userId);
                return Enumerable.Empty<UserMathProblem>();
            }
        }

        public async Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByTopicIdAsync(int topicId)
        {
            try
            {
                return await _context.UserMathProblems
                    .Where(p => p.TopicId == topicId)
                    .Include(p => p.User)
                    .Include(p => p.Topic)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for topic ID {TopicId}", topicId);
                return Enumerable.Empty<UserMathProblem>();
            }
        }

        public async Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByTopicNameAsync(string topicName)
        {
            try
            {
                return await _context.UserMathProblems
                    .Where(p => p.TopicName.Contains(topicName))
                    .Include(p => p.User)
                    .Include(p => p.Topic)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for topic name {TopicName}", topicName);
                return Enumerable.Empty<UserMathProblem>();
            }
        }

        public async Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByDifficultyAsync(DifficultyLevel difficulty)
        {
            try
            {
                return await _context.UserMathProblems
                    .Where(p => p.Difficulty == difficulty)
                    .Include(p => p.User)
                    .Include(p => p.Topic)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user math problems for difficulty {Difficulty}", difficulty);
                return Enumerable.Empty<UserMathProblem>();
            }
        }

        public async Task<UserMathProblem> CreateUserMathProblemAsync(UserMathProblem userMathProblem)
        {
            try
            {
                _context.UserMathProblems.Add(userMathProblem);
                await _context.SaveChangesAsync();
                return userMathProblem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user math problem for user {UserId}", userMathProblem.UserId);
                return null;
            }
        }

        public async Task<bool> UpdateUserMathProblemAsync(UserMathProblem userMathProblem)
        {
            try
            {
                _context.Entry(userMathProblem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user math problem with ID {ProblemId}", userMathProblem.Id);
                return false;
            }
        }

        public async Task<bool> DeleteUserMathProblemAsync(int id)
        {
            try
            {
                var userMathProblem = await _context.UserMathProblems.FindAsync(id);
                if (userMathProblem == null)
                {
                    return false;
                }

                _context.UserMathProblems.Remove(userMathProblem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user math problem with ID {ProblemId}", id);
                return false;
            }
        }
    }
} 