namespace MathTutor.Application.Constants
{
    /// <summary>
    /// Constants used in the AnswerEvaluationService
    /// </summary>
    public static class AnswerEvaluationConstants
    {
        /// <summary>
        /// Temperature value for answer evaluation AI model
        /// </summary>
        public const double AnswerEvaluationTemperature = 0.3;

        /// <summary>
        /// Maximum tokens for answer evaluation AI response
        /// </summary>
        public const int AnswerEvaluationMaxTokens = 500;

        /// <summary>
        /// Prompt template for evaluating math problem answers
        /// </summary>
        public const string AnswerEvaluationPromptTemplate = @"Evaluate if the following answer to the math problem is correct. Be helpful and educational, but also be forgiving and generous in your evaluation.

                Problem: {0}
                User's Answer: {1}

                Return the result as raw JSON with the following structure:
                {{
                    ""isCorrect"": true/false,
                    ""feedback"": ""Detailed feedback on the answer""
                }}

                Keep the feedback brief and under 200 characters. If the answer is partially correct, consider marking it as correct and provide guidance in the feedback.

                VERY IMPORTANT: Do not use markdown formatting. Do not wrap the JSON in code blocks or ```json tags. Only return the pure JSON object without any additional text or formatting. The first character should be '{{' and the last character should be '}}'.";

        /// <summary>
        /// Fallback message for correct answers
        /// </summary>
        public const string CorrectAnswerFeedback = "Your answer appears to be correct.";

        /// <summary>
        /// Fallback message for incorrect answers
        /// </summary>
        public const string IncorrectAnswerFeedback = "Your answer appears to be incorrect. Please check your work and try again.";
    }
}
