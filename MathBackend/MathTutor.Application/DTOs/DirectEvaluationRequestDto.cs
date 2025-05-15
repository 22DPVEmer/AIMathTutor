using System;

namespace MathTutor.Application.DTOs
{
    /// <summary>
    /// DTO for direct problem evaluation without using a stored problem
    /// </summary>
    public class DirectEvaluationRequestDto
    {
        /// <summary>
        /// The math problem statement to evaluate
        /// </summary>
        public string Problem { get; set; } = string.Empty;

        /// <summary>
        /// The user's answer to evaluate against the problem
        /// </summary>
        public string UserAnswer { get; set; } = string.Empty;
    }
} 