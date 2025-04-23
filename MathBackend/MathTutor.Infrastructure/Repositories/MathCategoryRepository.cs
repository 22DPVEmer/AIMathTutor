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
    public class MathCategoryRepository : IMathCategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MathCategoryRepository> _logger;

        public MathCategoryRepository(ApplicationDbContext context, ILogger<MathCategoryRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MathCategory>> GetAllCategoriesAsync()
        {
            try
            {
                return await _context.MathCategories
                    .Include(c => c.Topics)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories");
                return Enumerable.Empty<MathCategory>();
            }
        }

        public async Task<MathCategory> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _context.MathCategories
                    .Include(c => c.Topics)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
                return null;
            }
        }

        public async Task<MathCategory> CreateCategoryAsync(MathCategory category)
        {
            try
            {
                _context.MathCategories.Add(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {CategoryName}", category.Name);
                return null;
            }
        }

        public async Task<bool> UpdateCategoryAsync(MathCategory category)
        {
            try
            {
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", category.Id);
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.MathCategories.FindAsync(id);
                if (category == null)
                {
                    return false;
                }

                _context.MathCategories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
                return false;
            }
        }
    }
}