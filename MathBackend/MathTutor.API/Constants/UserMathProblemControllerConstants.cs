namespace MathTutor.API.Constants
{
    /// <summary>
    /// Constants used in the UserMathProblemController
    /// </summary>
    public static class UserMathProblemControllerConstants
    {
        /// <summary>
        /// Error messages for API responses
        /// </summary>
        public static class ErrorMessages
        {
            public const string UserNotAuthenticated = "User not authenticated";
            public const string FailedToCreateProblem = "Failed to create user math problem";
            public const string FailedToUpdateProblem = "Failed to update user math problem";
            public const string FailedToDeleteProblem = "Failed to delete user math problem";
            public const string FailedToUpdateWithNewAnswer = "Failed to update the problem with new answer";
            public const string ProblemNotFound = "User math problem with ID {0} not found";
            public const string TopicRequired = "Problem must be associated with a topic to be published";
            public const string EvaluationError = "Error evaluating answer: {0}";
        }

        /// <summary>
        /// Success messages for API responses
        /// </summary>
        public static class SuccessMessages
        {
            public const string PublishedSuccessfully = "Problem successfully published as a curated Math Problem";
        }

        /// <summary>
        /// Feedback messages for evaluation
        /// </summary>
        public static class FeedbackMessages
        {
            public const string CorrectAnswer = "Your answer is correct!";
            public const string IncorrectAnswer = "Your answer is incorrect. Please try again.";
        }

        /// <summary>
        /// Default values
        /// </summary>
        public static class DefaultValues
        {
            public const string DefaultProblemName = "{0} Problem";
        }
    }
} 