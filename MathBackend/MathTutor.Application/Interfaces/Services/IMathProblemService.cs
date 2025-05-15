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

        /// <summary>
        /// Evaluates a math problem answer and saves the attempt
        /// </summary>
        /// <param name="request">The evaluation and save request</param>
        /// <param name="userId">The ID of the user making the attempt</param>
        /// <returns>The evaluation result with additional information</returns>
        Task<EvaluateAndSaveResultDto> EvaluateAndSaveAsync(EvaluateAndSaveRequestDto request, string userId);

        /// <summary>
        /// Checks if a problem is a quadratic equation
        /// </summary>
        /// <param name="problem">The problem statement</param>
        /// <returns>True if the problem is a quadratic equation</returns>
        bool IsQuadraticEquation(string problem);

        /// <summary>
        /// Sanitizes an answer by removing whitespace and normalizing
        /// </summary>
        /// <param name="answer">The answer to sanitize</param>
        /// <returns>The sanitized answer</returns>
        string SanitizeAnswer(string answer);
    }
}