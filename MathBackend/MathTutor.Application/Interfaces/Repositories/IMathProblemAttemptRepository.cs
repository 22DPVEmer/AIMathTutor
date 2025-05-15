using MathTutor.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathProblemAttemptRepository
    {
        Task<IEnumerable<MathProblemAttempt>> GetAttemptsByUserIdAsync(string userId);
        Task<IEnumerable<MathProblemAttempt>> GetAttemptsByProblemIdAsync(int problemId);
        Task<IEnumerable<MathProblemAttempt>> GetAttemptsByUserIdAndProblemIdAsync(string userId, int problemId);
        Task<MathProblemAttempt> GetAttemptByIdAsync(int id);
        Task<MathProblemAttempt> CreateAttemptAsync(MathProblemAttempt attempt);
        Task<bool> CreateAttemptWithoutProblemAsync(MathProblemAttempt attempt, string problemStatement);
        Task<bool> UpdateAttemptAsync(MathProblemAttempt attempt);
        Task<bool> DeleteAttemptAsync(int id);
    }
}