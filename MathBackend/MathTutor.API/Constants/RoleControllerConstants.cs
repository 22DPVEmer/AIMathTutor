namespace MathTutor.API.Constants
{
    /// <summary>
    /// Constants used in the RoleController
    /// </summary>
    public static class RoleControllerConstants
    {
        /// <summary>
        /// Error messages for API responses
        /// </summary>
        public static class ErrorMessages
        {
            public const string RoleNameEmpty = "Role name cannot be empty";
            public const string RoleAlreadyExists = "Role '{0}' already exists";
            public const string FailedToCreateRole = "Failed to create role: {0}";
            public const string UserIdAndRoleRequired = "User ID and role name are required";
            public const string UserNotFound = "User not found";
            public const string RoleNotFound = "Role '{0}' not found";
            public const string UserAlreadyInRole = "User is already in role '{0}'";
            public const string FailedToAddUserToRole = "Failed to add user to role";
        }

        /// <summary>
        /// Success messages for API responses
        /// </summary>
        public static class SuccessMessages
        {
            public const string RoleCreated = "Role '{0}' created successfully";
            public const string UserAddedToRole = "User added to role '{0}' successfully";
        }

        /// <summary>
        /// Log messages
        /// </summary>
        public static class LogMessages
        {
            public const string RoleCreated = "Created new role: {RoleName}";
            public const string UserAddedToRole = "Added user {UserId} to role {RoleName}";
        }
    }
} 