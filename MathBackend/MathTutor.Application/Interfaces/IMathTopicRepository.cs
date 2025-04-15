using MathTutor.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathTopicRepository
    {
        Task<IEnumerable<MathTopic>> GetAllTopicsAsync();
        Task<MathTopic> GetTopicByIdAsync(int id);
        Task<IEnumerable<MathTopic>> GetTopicsByCategoryAsync(int categoryId);
        Task<MathTopic> CreateTopicAsync(MathTopic topic);
        Task<bool> UpdateTopicAsync(MathTopic topic);
        Task<bool> DeleteTopicAsync(int id);
    }
} 