using MathTutor.Core.Models;
using MathTutor.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathTopicService
    {
        Task<IEnumerable<MathTopicModel>> GetAllTopicsAsync();
        Task<MathTopicModel> GetTopicByIdAsync(int id);
        Task<IEnumerable<MathTopicModel>> GetTopicsBySchoolClassAsync(int schoolClassId);
        Task<MathTopicModel> CreateTopicAsync(MathTopicModel topicModel);
        Task<bool> UpdateTopicAsync(int id, MathTopicModel topicModel);
        Task<bool> DeleteTopicAsync(int id);

        // New method to get topic completion data for a user
        Task<IEnumerable<TopicCompletionDto>> GetTopicCompletionForUserAsync(string userId);
    }
}