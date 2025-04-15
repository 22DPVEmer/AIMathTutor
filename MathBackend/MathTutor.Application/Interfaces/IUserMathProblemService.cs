using MathTutor.Application.DTOs;
using MathTutor.Core.Enums;
using MathTutor.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IUserMathProblemService
    {
        Task<IEnumerable<UserMathProblemModel>> GetAllUserMathProblemsAsync();
        Task<UserMathProblemModel> GetUserMathProblemByIdAsync(int id);
        Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByUserIdAsync(string userId);
        Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByTopicIdAsync(int topicId);
        Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByTopicNameAsync(string topicName);
        Task<IEnumerable<UserMathProblemModel>> GetUserMathProblemsByDifficultyAsync(string difficulty);
        Task<UserMathProblemModel> CreateUserMathProblemAsync(UserMathProblemModel userMathProblemModel);
        Task<bool> UpdateUserMathProblemAsync(int id, UserMathProblemModel userMathProblemModel);
        Task<bool> DeleteUserMathProblemAsync(int id);
        Task<UserMathProblemModel> SaveGeneratedProblemAsync(SaveProblemAttemptDto problemAttemptDto);
    }
} 