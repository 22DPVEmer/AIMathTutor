using MathTutor.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathTopicService
    {
        Task<IEnumerable<MathTopicModel>> GetAllTopicsAsync();
        Task<MathTopicModel> GetTopicByIdAsync(int id);
        Task<IEnumerable<MathTopicModel>> GetTopicsByCategoryAsync(int categoryId);
        Task<MathTopicModel> CreateTopicAsync(MathTopicModel topicModel);
        Task<bool> UpdateTopicAsync(int id, MathTopicModel topicModel);
        Task<bool> DeleteTopicAsync(int id);
    }
} 