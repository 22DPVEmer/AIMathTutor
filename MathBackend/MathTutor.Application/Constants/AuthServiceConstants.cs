namespace MathTutor.Application.Constants
{
    /// <summary>
    /// Constants used in the AuthService
    /// </summary>
    public static class AuthServiceConstants
    {
        // Registration messages
        public const string UserAlreadyExistsMessage = "User already exists";
        public const string EmailAlreadyRegisteredError = "Email is already registered";
        public const string UserCreationFailedMessage = "User creation failed";
        public const string FailedToCreateUserError = "Failed to create user";
        public const string RegistrationSuccessMessage = "User registered successfully. Please verify your email address.";
        public const string RegistrationFailedMessage = "Registration failed";
        public const string RegistrationErrorMessage = "An error occurred during registration";
        
        // Default role
        public const string DefaultUserRole = "User";

        // Login messages
        public const string InvalidLoginAttemptMessage = "Invalid login attempt";
        public const string InvalidCredentialsError = "Invalid email or password";
        public const string LoginSuccessMessage = "Login successful";
        public const string LoginFailedMessage = "Login failed";
        public const string LoginErrorMessage = "An error occurred during login";

        // Token messages
        public const string InvalidTokenMessage = "Invalid token";
        public const string TokenValidationFailedError = "Token validation failed";
        public const string UserNotFoundMessage = "User not found";
        public const string UserNotFoundWithTokenError = "User associated with token not found";
        public const string TokenRefreshedMessage = "Token refreshed successfully";
        public const string TokenRefreshFailedMessage = "Token refresh failed";
        public const string TokenRefreshErrorMessage = "An error occurred while refreshing the token";
    }
}
