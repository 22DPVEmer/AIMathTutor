namespace MathTutor.API.Constants
{
    /// <summary>
    /// Constants used in the UserController
    /// </summary>
    public static class UserControllerConstants
    {
        /// <summary>
        /// Error messages for API responses
        /// </summary>
        public static class ErrorMessages
        {
            public const string NamesRequired = "First name and last name are required";
            public const string UserNotFoundOrUpdateFailed = "User not found or update failed";
            public const string UpdateError = "An error occurred while updating the user profile";
            public const string PasswordsRequired = "Current password and new password are required";
            public const string PasswordTooShort = "New password must be at least 6 characters long";
            public const string PasswordChangeFailed = "Failed to change password. Please ensure your current password is correct.";
            public const string DeleteFailed = "Failed to delete user";
        }

        /// <summary>
        /// Logging messages
        /// </summary>
        public static class LogMessages
        {
            public const string ProfileRetrieved = "User profile retrieved for user ID: {0}";
            public const string ProfileUpdated = "User profile updated successfully for user ID: {0}";
            public const string UpdateError = "Error updating user profile for user ID: {0}";
            public const string GetProfileCalled = "GetProfile called";
        }
    }
} 