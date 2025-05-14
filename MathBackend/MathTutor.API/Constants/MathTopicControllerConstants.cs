namespace MathTutor.API.Constants
{
    /// <summary>
    /// Constants used in the MathTopicController
    /// </summary>
    public static class MathTopicControllerConstants
    {
        /// <summary>
        /// Error messages for API responses
        /// </summary>
        public static class ErrorMessages
        {
            public const string FailedToCreateTopic = "Failed to create topic";
            public const string FailedToUpdateTopic = "Failed to update topic";
            public const string FailedToDeleteTopic = "Failed to delete topic";
            public const string UserNotAuthenticated = "User not authenticated";
        }
    }
} 