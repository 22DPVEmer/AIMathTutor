using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathProblemRepository
    {
        Task<IEnumerable<MathProblem>> GetAllProblemsAsync();
        Task<MathProblem> GetProblemByIdAsync(int id);
        Task<IEnumerable<MathProblem>> GetProblemsByTopicAsync(int topicId);
        Task<IEnumerable<MathProblem>> GetProblemsByDifficultyAsync(DifficultyLevel difficulty);
        Task<IEnumerable<MathProblem>> GetProblemsByTopicAndDifficultyAsync(int topicId, DifficultyLevel difficulty);
        Task<MathProblem> CreateProblemAsync(MathProblem problem);
        Task<bool> UpdateProblemAsync(MathProblem problem);
        Task<bool> DeleteProblemAsync(int id);
    }
} 