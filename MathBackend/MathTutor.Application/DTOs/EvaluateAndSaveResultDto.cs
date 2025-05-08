using System.Collections.Generic;
using MathTutor.Core.Models;

namespace MathTutor.Application.DTOs
{
    /// <summary>
    /// DTO for the result of evaluating and saving a math problem attempt
    /// </summary>
    public class EvaluateAndSaveResultDto
    {
        /// <summary>
        /// Whether the save operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Whether the answer was correct
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Feedback about the answer
        /// </summary>
        public string Feedback { get; set; } = string.Empty;

        /// <summary>
        /// Whether the user already had a correct attempt for this problem
        /// </summary>
        public bool HasExistingCorrectAttempt { get; set; }

        /// <summary>
        /// Updated list of problems for the topic (optional)
        /// </summary>
        public IEnumerable<MathProblemModel>? Problems { get; set; }

        /// <summary>
        /// Updated list of attempts for the user (optional)
        /// </summary>
        public IEnumerable<MathProblemAttemptModel>? Attempts { get; set; }
    }
}
