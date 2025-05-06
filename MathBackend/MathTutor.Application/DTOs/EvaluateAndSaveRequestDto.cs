using System;

namespace MathTutor.Application.DTOs
{
    /// <summary>
    /// DTO for combined problem evaluation and saving in one request
    /// </summary>
    public class EvaluateAndSaveRequestDto
    {
        /// <summary>
        /// The math problem statement to evaluate
        /// </summary>
        public string Problem { get; set; } = string.Empty;

        /// <summary>
        /// The user's answer to evaluate against the problem
        /// </summary>
        public string UserAnswer { get; set; } = string.Empty;

        /// <summary>
        /// The solution to the problem (optional, used for saving)
        /// </summary>
        public string Solution { get; set; } = string.Empty;

        /// <summary>
        /// The explanation for the solution (optional, used for saving)
        /// </summary>
        public string Explanation { get; set; } = string.Empty;

        /// <summary>
        /// The name of the problem (optional, used for saving)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The difficulty level of the problem (optional, used for saving)
        /// </summary>
        public string Difficulty { get; set; } = string.Empty;

        /// <summary>
        /// The topic of the problem (optional, used for saving)
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// The topic ID of the problem (optional, used for saving)
        /// </summary>
        public int? TopicId { get; set; }
    }
}
