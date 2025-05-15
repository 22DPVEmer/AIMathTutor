namespace MathTutor.Application.Constants
{
    public static class ProblemGenerationServiceConstants
    {
        public static class ModelParameters
        {
            public const double Temperature = 0.7;
            public const int MaxTokens = 800;
        }

        public static class RequiredProperties
        {
            public const string Statement = "statement";
            public const string Solution = "solution";
            public const string Explanation = "explanation";
        }

        public const string ProblemGenerationPromptTemplate = @"Generate a math problem on the topic of {0} with {1} difficulty level.
                Return the result as raw JSON with the following structure:
                {{
                    ""statement"": ""The problem statement"",
                    ""solution"": ""The correct answer"",
                    ""explanation"": ""Step-by-step explanation of how to solve the problem""
                }}

                Keep the explanation brief and under 250 characters.
                VERY IMPORTANT: Do not use markdown formatting. Do not wrap the JSON in code blocks or ```json tags. Only return the pure JSON object without any additional text or formatting. The first character should be '{{' and the last character should be '}}'.";

        public const string FallbackProblemTemplate = @"{{
                    ""statement"": ""Basic {0} problem: What is 5 + 7?"",
                    ""solution"": ""12"",
                    ""explanation"": ""Add the numbers: 5 + 7 = 12. This is a fallback problem because the system could not generate a custom problem.""
                }}";
    }
}
