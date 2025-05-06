using System.Collections.Generic;
using MathTutor.Core.Models;

namespace MathTutor.Application.DTOs
{
    /// <summary>
    /// DTO for the response from the combined evaluate and save endpoint
    /// </summary>
    public class EvaluateAndSaveResponseDto
    {
        /// <summary>
        /// Whether the evaluation and saving was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Whether the answer was correct
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Feedback on the answer
        /// </summary>
        public string Feedback { get; set; } = string.Empty;

        /// <summary>
        /// Whether the user already had a correct attempt for this problem
        /// </summary>
        public bool HasExistingCorrectAttempt { get; set; }

        /// <summary>
        /// Updated list of problems for the topic (if a topic was specified)
        /// </summary>
        public IEnumerable<MathProblemModel>? Problems { get; set; }

        /// <summary>
        /// Updated list of attempts for the user
        /// </summary>
        public IEnumerable<MathProblemAttemptModel>? Attempts { get; set; }
    }
}
