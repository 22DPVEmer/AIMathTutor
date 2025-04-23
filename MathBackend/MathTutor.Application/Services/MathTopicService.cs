using AutoMapper;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    public class MathTopicService : IMathTopicService
    {
        private readonly IMathTopicRepository _mathTopicRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MathTopicService> _logger;

        public MathTopicService(
            IMathTopicRepository mathTopicRepository,
            IMapper mapper,
            ILogger<MathTopicService> logger)
        {
            _mathTopicRepository = mathTopicRepository ?? throw new ArgumentNullException(nameof(mathTopicRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MathTopicModel>> GetAllTopicsAsync()
        {
            try
            {
                var topics = await _mathTopicRepository.GetAllTopicsAsync();
                var topicModels = _mapper.Map<IEnumerable<MathTopicModel>>(topics).ToList();
                
                // Organize into hierarchy
                var rootTopics = topicModels.Where(t => !t.ParentTopicId.HasValue).ToList();
                var childTopics = topicModels.Where(t => t.ParentTopicId.HasValue).ToList();
                
                // Build the hierarchy
                foreach (var root in rootTopics)
                {
                    BuildTopicHierarchy(root, childTopics);
                }
                
                return rootTopics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all topics");
                return Enumerable.Empty<MathTopicModel>();
            }
        }
        
        private void BuildTopicHierarchy(MathTopicModel parent, List<MathTopicModel> allChildren)
        {
            var children = allChildren.Where(t => t.ParentTopicId == parent.Id).ToList();
            parent.Subtopics.AddRange(children);
            
            foreach (var child in children)
            {
                BuildTopicHierarchy(child, allChildren);
            }
        }

        public async Task<MathTopicModel> GetTopicByIdAsync(int id)
        {
            try
            {
                var topic = await _mathTopicRepository.GetTopicByIdAsync(id);
                return _mapper.Map<MathTopicModel>(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting topic with ID {TopicId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<MathTopicModel>> GetTopicsByCategoryAsync(int categoryId)
        {
            try
            {
                var topics = await _mathTopicRepository.GetTopicsByCategoryAsync(categoryId);
                return _mapper.Map<IEnumerable<MathTopicModel>>(topics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting topics for category ID {CategoryId}", categoryId);
                return Enumerable.Empty<MathTopicModel>();
            }
        }

        public async Task<MathTopicModel> CreateTopicAsync(MathTopicModel topicModel)
        {
            try
            {
                var topic = _mapper.Map<MathTopic>(topicModel);
                var createdTopic = await _mathTopicRepository.CreateTopicAsync(topic);
                return _mapper.Map<MathTopicModel>(createdTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating topic {TopicName}", topicModel.Name);
                return null;
            }
        }

        public async Task<bool> UpdateTopicAsync(int id, MathTopicModel topicModel)
        {
            try
            {
                var existingTopic = await _mathTopicRepository.GetTopicByIdAsync(id);
                if (existingTopic == null)
                {
                    return false;
                }

                _mapper.Map(topicModel, existingTopic);
                existingTopic.Id = id; // Ensure ID is maintained
                
                return await _mathTopicRepository.UpdateTopicAsync(existingTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topic with ID {TopicId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            try
            {
                return await _mathTopicRepository.DeleteTopicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting topic with ID {TopicId}", id);
                return false;
            }
        }
    }
} 