namespace MathTutor.API.Constants
{
    public static class MathProblemControllerConstants
    {
        public static class ErrorMessages
        {
            public const string InvalidTopicId = "Invalid topic ID";
            public const string RequestBodyRequired = "Request body is required";
            public const string MissingProblemOrAnswer = "Problem statement and user answer are required";
            public const string QuadraticEquationDetected = "Detected quadratic equation. Using AI evaluation for quadratic equation.";
            public const string FailedAiResponse = "Failed to get a valid response from the AI service";
            public const string ParseError = "Failed to parse the response: {0}";
            public const string InvalidGuidanceRequest = "Problem statement is required for guidance";
            public const string FailedGuidanceResponse = "Failed to get guidance from the AI service";
            public const string FailedToSaveAttempt = "Failed to save problem attempt";
            public const string FailedToEvaluateAndSave = "Failed to evaluate and save the answer";
        }

        public static class LogMessages
        {
            public const string TopicErrorFormat = "Error fetching problems for topic {0}: {1}";
            public const string EvaluationRequestReceived = "Received evaluation request: Problem={0} chars, UserAnswer={1} chars";
            public const string AiResponseReceived = "AI response for evaluation: {0} chars";
            public const string GuidanceRequestReceived = "Received guidance request: Problem={0} chars";
            public const string GuidanceResponseReceived = "AI response for guidance: {0} chars";
            public const string AttemptSaveRequested = "Saving problem attempt for user {0}";
            public const string EvaluationRequested = "Evaluating answer for user {0}";
        }
    }
}