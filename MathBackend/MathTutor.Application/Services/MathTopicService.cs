using AutoMapper;
using MathTutor.Application.Constants;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    public class MathTopicService : IMathTopicService
    {
        private readonly IMathTopicRepository _mathTopicRepository;
        private readonly IMathProblemRepository _mathProblemRepository;
        private readonly IMathProblemAttemptRepository _mathProblemAttemptRepository;
        private readonly IMapper _mapper;

        public MathTopicService(
            IMathTopicRepository mathTopicRepository,
            IMathProblemRepository mathProblemRepository,
            IMathProblemAttemptRepository mathProblemAttemptRepository,
            IMapper mapper)
        {
            _mathTopicRepository = mathTopicRepository ?? throw new ArgumentNullException(nameof(mathTopicRepository));
            _mathProblemRepository = mathProblemRepository ?? throw new ArgumentNullException(nameof(mathProblemRepository));
            _mathProblemAttemptRepository = mathProblemAttemptRepository ?? throw new ArgumentNullException(nameof(mathProblemAttemptRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            catch
            {
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
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<MathTopicModel>> GetTopicsBySchoolClassAsync(int schoolClassId)
        {
            try
            {
                var topics = await _mathTopicRepository.GetTopicsBySchoolClassAsync(schoolClassId);
                return _mapper.Map<IEnumerable<MathTopicModel>>(topics);
            }
            catch
            {
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
            catch
            {
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
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            try
            {
                return await _mathTopicRepository.DeleteTopicAsync(id);
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<TopicCompletionDto>> GetTopicCompletionForUserAsync(string userId)
        {
            try
            {
                // Get all topics
                var topics = await _mathTopicRepository.GetAllTopicsAsync();

                // Get all user attempts in one query for efficiency
                var userAttempts = await _mathProblemAttemptRepository.GetAttemptsByUserIdAsync(userId);

                // Get all problems in one query for efficiency
                var allProblems = await _mathProblemRepository.GetAllProblemsAsync();

                // Group problems by topic for faster lookup
                var problemsByTopic = allProblems.GroupBy(p => p.TopicId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var result = new List<TopicCompletionDto>();

                // Process each topic
                foreach (var topic in topics)
                {
                    // Get problems for this topic from our dictionary
                    var problems = problemsByTopic.ContainsKey(topic.Id)
                        ? problemsByTopic[topic.Id]
                        : new List<MathProblem>();

                    // Calculate total points possible
                    int totalPointsPossible = problems.Sum(p => p.PointValue);

                    // Calculate points earned
                    int pointsEarned = 0;

                    // Create a lookup of problem IDs for faster matching
                    var problemIds = problems.Select(p => p.Id).ToHashSet();

                    // Create a dictionary to map normalized statements to problem IDs
                    var statementToProblemIds = new Dictionary<string, List<int>>();

                    // Build the mapping of statements to problem IDs
                    foreach (var problem in problems)
                    {
                        var normalizedStatement = problem.Statement.ToLower().Replace(MathTopicServiceConstants.StatementNormalization.WhitespaceCharacter, MathTopicServiceConstants.StatementNormalization.EmptyReplacement);
                        if (!statementToProblemIds.ContainsKey(normalizedStatement))
                        {
                            statementToProblemIds[normalizedStatement] = new List<int>();
                        }
                        statementToProblemIds[normalizedStatement].Add(problem.Id);
                    }

                    // Find all successful attempts for problems in this topic
                    // First, get attempts that match by problem ID
                    var topicAttemptsByProblemId = userAttempts
                        .Where(a => problemIds.Contains(a.ProblemId) && a.IsCorrect)
                        .GroupBy(a => a.ProblemId)
                        .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.PointsEarned).First());

                    // Track which problems we've already counted to avoid double-counting
                    var countedProblemIds = topicAttemptsByProblemId.Keys.ToHashSet();

                    // Track which statements we've already counted
                    var countedStatements = new HashSet<string>();

                    // Find additional successful attempts by matching problem statements
                    var additionalAttempts = new List<MathProblemAttempt>();

                    // Process attempts with problems
                    foreach (var attempt in userAttempts.Where(a => a.IsCorrect && a.Problem != null))
                    {
                        // Skip if we've already counted this problem
                        if (countedProblemIds.Contains(attempt.ProblemId))
                            continue;

                        // Normalize the statement
                        var normalizedStatement = attempt.Problem.Statement.ToLower().Replace(MathTopicServiceConstants.StatementNormalization.WhitespaceCharacter, MathTopicServiceConstants.StatementNormalization.EmptyReplacement);

                        // Skip if we've already counted this statement
                        if (countedStatements.Contains(normalizedStatement))
                            continue;

                        // Check if this statement maps to any problems in this topic
                        if (statementToProblemIds.ContainsKey(normalizedStatement))
                        {
                            // Add the attempt
                            additionalAttempts.Add(attempt);

                            // Mark the statement as counted
                            countedStatements.Add(normalizedStatement);

                            // Mark all problems with this statement as counted
                            foreach (var problemId in statementToProblemIds[normalizedStatement])
                            {
                                countedProblemIds.Add(problemId);
                            }
                        }
                    }

                    // Combine the attempts
                    var topicAttempts = topicAttemptsByProblemId.Values.Concat(additionalAttempts);

                    // Sum up the points earned
                    pointsEarned = topicAttempts.Sum(a => a.PointsEarned);

                    // Calculate percentage
                    int percentageCompleted = totalPointsPossible > 0
                        ? (int)Math.Round((double)pointsEarned / totalPointsPossible * MathTopicServiceConstants.PercentageCalculation.PercentageMultiplier)
                        : MathTopicServiceConstants.PercentageCalculation.DefaultPercentage;

                    // Add to result
                    result.Add(new TopicCompletionDto
                    {
                        TopicId = topic.Id,
                        TopicName = topic.Name,
                        TotalPointsPossible = totalPointsPossible,
                        PointsEarned = pointsEarned,
                        PercentageCompleted = percentageCompleted
                    });
                }

                return result;
            }
            catch (Exception)
            {
                return Enumerable.Empty<TopicCompletionDto>();
            }
        }
    }
}