namespace MathTutor.API.Constants
{
    /// <summary>
    /// Constants used in the MathProblemAttemptController
    /// </summary>
    public static class MathProblemAttemptControllerConstants
    {
        /// <summary>
        /// Error messages for API responses
        /// </summary>
        public static class ErrorMessages
        {
            public const string UserNotAuthenticated = "User not authenticated";
            public const string AttemptNotFound = "Attempt with ID {0} not found";
            public const string NotAuthorizedToViewAttempt = "You are not authorized to view this attempt";
        }
    }
} 