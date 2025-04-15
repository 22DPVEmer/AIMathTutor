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
    public class MathTopicRepository : IMathTopicRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MathTopicRepository> _logger;

        public MathTopicRepository(ApplicationDbContext context, ILogger<MathTopicRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MathTopic>> GetAllTopicsAsync()
        {
            try
            {
                return await _context.MathTopics
                    .Include(t => t.Category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all topics");
                return Enumerable.Empty<MathTopic>();
            }
        }

        public async Task<MathTopic> GetTopicByIdAsync(int id)
        {
            try
            {
                return await _context.MathTopics
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving topic with ID {TopicId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<MathTopic>> GetTopicsByCategoryAsync(int categoryId)
        {
            try
            {
                return await _context.MathTopics
                    .Where(t => t.CategoryId == categoryId)
                    .Include(t => t.Category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving topics for category ID {CategoryId}", categoryId);
                return Enumerable.Empty<MathTopic>();
            }
        }

        public async Task<MathTopic> CreateTopicAsync(MathTopic topic)
        {
            try
            {
                _context.MathTopics.Add(topic);
                await _context.SaveChangesAsync();
                return topic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating topic {TopicName}", topic.Name);
                return null;
            }
        }

        public async Task<bool> UpdateTopicAsync(MathTopic topic)
        {
            try
            {
                _context.Entry(topic).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topic with ID {TopicId}", topic.Id);
                return false;
            }
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            try
            {
                var topic = await _context.MathTopics.FindAsync(id);
                if (topic == null)
                {
                    return false;
                }

                _context.MathTopics.Remove(topic);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting topic with ID {TopicId}", id);
                return false;
            }
        }
    }
} 