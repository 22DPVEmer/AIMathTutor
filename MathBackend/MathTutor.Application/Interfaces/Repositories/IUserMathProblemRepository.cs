using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IUserMathProblemRepository
    {
        Task<IEnumerable<UserMathProblem>> GetAllUserMathProblemsAsync();
        Task<UserMathProblem> GetUserMathProblemByIdAsync(int id);
        Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByUserIdAsync(string userId);
        Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByTopicIdAsync(int topicId);
        Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByTopicNameAsync(string topicName);
        Task<IEnumerable<UserMathProblem>> GetUserMathProblemsByDifficultyAsync(DifficultyLevel difficulty);
        Task<UserMathProblem> CreateUserMathProblemAsync(UserMathProblem userMathProblem);
        Task<bool> UpdateUserMathProblemAsync(UserMathProblem userMathProblem);
        Task<bool> DeleteUserMathProblemAsync(int id);
    }
} 