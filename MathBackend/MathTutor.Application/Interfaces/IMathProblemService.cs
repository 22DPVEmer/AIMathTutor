using MathTutor.Application.DTOs;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathTutor.Core.Models;

namespace MathTutor.Application.Interfaces
{
    public interface IMathProblemService
    {
        Task<IEnumerable<MathProblemModel>> GetAllProblemsAsync();
        Task<MathProblemModel> GetProblemByIdAsync(int id);
        Task<IEnumerable<MathProblemModel>> GetProblemsByTopicAsync(int topicId);
        Task<IEnumerable<MathProblemModel>> GetProblemsByDifficultyAsync(DifficultyLevel difficulty);
        Task<IEnumerable<MathProblemModel>> GetProblemsByTopicAndDifficultyAsync(int topicId, DifficultyLevel difficulty);
        Task<MathProblemModel> CreateProblemAsync(CreateMathProblemDto problemDto);
        Task<bool> UpdateProblemAsync(int id, UpdateMathProblemDto problemDto);
        Task<bool> DeleteProblemAsync(int id);
        Task<GeneratedMathProblemResponseDto> GenerateMathProblemAsync(GenerateMathProblemRequestDto request);
        Task<EvaluateMathAnswerResponseDto> EvaluateAnswerAsync(EvaluateMathAnswerRequestDto request);
        Task<bool> SaveProblemAttemptAsync(SaveProblemAttemptDto attemptDto);
        Task<IEnumerable<MathProblemAttemptModel>> GetAttemptsByUserIdAsync(string userId);
    }
}