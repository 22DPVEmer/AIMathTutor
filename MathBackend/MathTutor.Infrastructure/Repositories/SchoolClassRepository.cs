using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MathTutor.Infrastructure.Repositories;

public class SchoolClassRepository : ISchoolClassRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SchoolClassRepository> _logger;

    public SchoolClassRepository(ApplicationDbContext context, ILogger<SchoolClassRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SchoolClass>> GetAllClassesAsync()
    {
        try
        {
            return await _context.SchoolClasses
                .Include(sc => sc.Topics)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all school classes");
            throw;
        }
    }

    public async Task<SchoolClass?> GetClassByIdAsync(int id)
    {
        try
        {
            return await _context.SchoolClasses
                .Include(sc => sc.Topics)
                .FirstOrDefaultAsync(sc => sc.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting school class by ID: {Id}", id);
            throw;
        }
    }

    public async Task<SchoolClass> CreateClassAsync(SchoolClass schoolClass)
    {
        try
        {
            await _context.SchoolClasses.AddAsync(schoolClass);
            await _context.SaveChangesAsync();
            return schoolClass;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating school class");
            throw;
        }
    }

    public async Task<SchoolClass> UpdateClassAsync(SchoolClass schoolClass)
    {
        try
        {
            _context.SchoolClasses.Update(schoolClass);
            await _context.SaveChangesAsync();
            return schoolClass;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating school class: {Id}", schoolClass.Id);
            throw;
        }
    }

    public async Task DeleteClassAsync(int id)
    {
        try
        {
            var schoolClass = await _context.SchoolClasses.FindAsync(id);
            if (schoolClass != null)
            {
                _context.SchoolClasses.Remove(schoolClass);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting school class: {Id}", id);
            throw;
        }
    }

    public async Task<int> GetTopicCountAsync(int schoolClassId)
    {
        try
        {
            return await _context.MathTopics
                .CountAsync(t => t.SchoolClassId == schoolClassId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting topic count for school class: {Id}", schoolClassId);
            throw;
        }
    }
} 