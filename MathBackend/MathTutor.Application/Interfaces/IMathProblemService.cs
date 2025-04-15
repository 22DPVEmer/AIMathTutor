using MathTutor.Application.DTOs;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathProblemService
    {
        Task<IEnumerable<MathProblemDto>> GetAllProblemsAsync();
        Task<MathProblemDto> GetProblemByIdAsync(int id);
        Task<IEnumerable<MathProblemDto>> GetProblemsByTopicAsync(int topicId);
        Task<IEnumerable<MathProblemDto>> GetProblemsByDifficultyAsync(DifficultyLevel difficulty);
        Task<IEnumerable<MathProblemDto>> GetProblemsByTopicAndDifficultyAsync(int topicId, DifficultyLevel difficulty);
        Task<MathProblemDto> CreateProblemAsync(CreateMathProblemDto problemDto);
        Task<bool> UpdateProblemAsync(int id, UpdateMathProblemDto problemDto);
        Task<bool> DeleteProblemAsync(int id);
        Task<GeneratedMathProblemResponseDto> GenerateMathProblemAsync(GenerateMathProblemRequestDto request);
        Task<EvaluateMathAnswerResponseDto> EvaluateAnswerAsync(EvaluateMathAnswerRequestDto request);
        Task<bool> SaveProblemAttemptAsync(SaveProblemAttemptDto attemptDto);
    }
} 