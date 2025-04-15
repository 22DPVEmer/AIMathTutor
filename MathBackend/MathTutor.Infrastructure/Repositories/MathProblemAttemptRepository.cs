using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathTutor.Infrastructure.Repositories
{
    public class MathProblemAttemptRepository : IMathProblemAttemptRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MathProblemAttemptRepository> _logger;

        public MathProblemAttemptRepository(ApplicationDbContext context, ILogger<MathProblemAttemptRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MathProblemAttempt>> GetAttemptsByUserIdAsync(string userId)
        {
            try
            {
                return await _context.MathProblemAttempts
                    .Where(a => a.UserId == userId)
                    .Include(a => a.Problem)
                    .OrderByDescending(a => a.AttemptedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving attempts for user {UserId}", userId);
                return Enumerable.Empty<MathProblemAttempt>();
            }
        }

        public async Task<IEnumerable<MathProblemAttempt>> GetAttemptsByProblemIdAsync(int problemId)
        {
            try
            {
                return await _context.MathProblemAttempts
                    .Where(a => a.ProblemId == problemId)
                    .Include(a => a.User)
                    .OrderByDescending(a => a.AttemptedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving attempts for problem {ProblemId}", problemId);
                return Enumerable.Empty<MathProblemAttempt>();
            }
        }

        public async Task<MathProblemAttempt> GetAttemptByIdAsync(int id)
        {
            try
            {
                return await _context.MathProblemAttempts
                    .Include(a => a.Problem)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving attempt with ID {AttemptId}", id);
                return null;
            }
        }

        public async Task<MathProblemAttempt> CreateAttemptAsync(MathProblemAttempt attempt)
        {
            try
            {
                _context.MathProblemAttempts.Add(attempt);
                await _context.SaveChangesAsync();
                return attempt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating attempt for user {UserId} and problem {ProblemId}", 
                    attempt.UserId, attempt.ProblemId);
                return null;
            }
        }
        
        public async Task<bool> CreateAttemptWithoutProblemAsync(MathProblemAttempt attempt, string problemStatement)
        {
            try
            {
                // Set the problem ID to 0 to indicate it's not linked to a persistent problem
                attempt.ProblemId = 0;
                
                // Store the attempt
                _context.MathProblemAttempts.Add(attempt);
                await _context.SaveChangesAsync();
                
                // Here you could also save the problem statement in another table or as metadata
                // For now, we'll just log it
                _logger.LogInformation("Saved attempt without persistent problem. Problem statement: {Statement}", 
                    problemStatement);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating attempt without problem for user {UserId}", attempt.UserId);
                return false;
            }
        }

        public async Task<bool> UpdateAttemptAsync(MathProblemAttempt attempt)
        {
            try
            {
                _context.Entry(attempt).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating attempt with ID {AttemptId}", attempt.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAttemptAsync(int id)
        {
            try
            {
                var attempt = await _context.MathProblemAttempts.FindAsync(id);
                if (attempt == null)
                {
                    return false;
                }

                _context.MathProblemAttempts.Remove(attempt);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attempt with ID {AttemptId}", id);
                return false;
            }
        }
    }
} 