namespace MathTutor.API.Constants
{
    public static class BaseApiControllerConstants
    {
        public static class ErrorMessages
        {
            public const string EmptyAiResponse = "AI service returned an empty response";
            public const string InvalidAiResponseFormat = "Failed to parse AI response due to invalid format";
            public const string GenericAiParseError = "Failed to parse AI response: {0}";
        }
    }
}