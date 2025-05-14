namespace MathTutor.API.Constants
{
    /// <summary>
    /// Constants used in the SchoolClassController
    /// </summary>
    public static class SchoolClassControllerConstants
    {
        /// <summary>
        /// Error messages for API responses
        /// </summary>
        public static class ErrorMessages
        {
            public const string ClassNotFound = "School class with ID {0} not found";
            public const string IdMismatch = "ID mismatch";
            public const string ServerError = "An error occurred while {0}";
        }

        /// <summary>
        /// Log messages
        /// </summary>
        public static class LogMessages
        {
            public const string ErrorRetrievingClasses = "Error retrieving school classes";
            public const string ErrorRetrievingClass = "Error retrieving school class with ID {ClassId}";
            public const string ErrorCreatingClass = "Error creating school class";
            public const string ErrorUpdatingClass = "Error updating school class with ID {ClassId}";
            public const string ErrorDeletingClass = "Error deleting school class with ID {ClassId}";
        }
    }
} 