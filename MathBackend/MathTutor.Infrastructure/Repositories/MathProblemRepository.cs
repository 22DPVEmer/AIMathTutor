using Microsoft.EntityFrameworkCore;
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
    public class MathProblemRepository : IMathProblemRepository
    {
        private readonly ApplicationDbContext _context;

        public MathProblemRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<MathProblem>> GetAllProblemsAsync()
        {
            return await _context.MathProblems
                .Include(p => p.Topic)
                .ToListAsync();
        }

        public async Task<MathProblem> GetProblemByIdAsync(int id)
        {
            return await _context.MathProblems
                .Include(p => p.Topic)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<MathProblem>> GetProblemsByTopicAsync(int topicId)
        {
            return await _context.MathProblems
                .Include(p => p.Topic)
                .Where(p => p.TopicId == topicId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MathProblem>> GetProblemsByDifficultyAsync(DifficultyLevel difficulty)
        {
            return await _context.MathProblems
                .Include(p => p.Topic)
                .Where(p => p.Difficulty == difficulty)
                .ToListAsync();
        }

        public async Task<IEnumerable<MathProblem>> GetProblemsByTopicAndDifficultyAsync(int topicId, DifficultyLevel difficulty)
        {
            return await _context.MathProblems
                .Include(p => p.Topic)
                .Where(p => p.TopicId == topicId && p.Difficulty == difficulty)
                .ToListAsync();
        }

        public async Task<MathProblem> CreateProblemAsync(MathProblem problem)
        {
            await _context.MathProblems.AddAsync(problem);
            await _context.SaveChangesAsync();
            return problem;
        }

        public async Task<bool> UpdateProblemAsync(MathProblem problem)
        {
            _context.Entry(problem).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MathProblemExists(problem.Id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteProblemAsync(int id)
        {
            var problem = await _context.MathProblems.FindAsync(id);
            
            if (problem == null)
            {
                return false;
            }

            _context.MathProblems.Remove(problem);
            await _context.SaveChangesAsync();
            
            return true;
        }

        private async Task<bool> MathProblemExists(int id)
        {
            return await _context.MathProblems.AnyAsync(e => e.Id == id);
        }
    }
} 