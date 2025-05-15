namespace MathTutor.Application.DTOs
{
    /// <summary>
    /// DTO for requesting a math problem retry with a new answer
    /// </summary>
    public class RetryProblemRequestDto
    {
        /// <summary>
        /// The user's answer to the math problem
        /// </summary>
        public string UserAnswer { get; set; } = string.Empty;
    }
} 