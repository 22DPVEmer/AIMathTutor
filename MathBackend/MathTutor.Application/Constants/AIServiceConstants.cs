using System;

namespace MathTutor.Application.Constants
{
    public static class AIServiceConstants
    {
        public const string GenerateResponseErrorMessage = "I'm sorry, I couldn't generate a response at this time.";

        public const string MathProblemGenerationFallbackJson = @"{
            ""statement"": ""Simple math problem: Unable to generate a custom problem at this time."",
            ""solution"": ""Contact your teacher for assistance."",
            ""explanation"": ""The system encountered an error while generating this problem.""
        }";

        public const string AnswerEvaluationFallbackJson = @"{
            ""isCorrect"": false,
            ""feedback"": ""Unable to evaluate your answer at this time. Please try again later or contact your instructor for assistance.""
        }";

        public const string GuidanceFallbackJson = @"{
            ""guidance"": ""I recommend reviewing the problem step-by-step. Break it down into smaller parts and solve each part separately. Check your calculations carefully and make sure you understand the concepts involved.""
        }";
    }
}
