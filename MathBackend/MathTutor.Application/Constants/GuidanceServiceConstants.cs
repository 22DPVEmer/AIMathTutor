using System.Text.RegularExpressions;

namespace MathTutor.Application.Constants
{
    public static class GuidanceServiceConstants
    {
        public const double GuidanceTemperature = 0.5;
        public const int GuidanceMaxTokens = 600;
        public const int MaxGuidanceLength = 500;

        public const string GuidancePromptTemplate = @"Provide guidance for a student who is working on this math problem:
                Problem: {0}
                Correct Solution: {1}
                Student's Answer: {2}
                Student's Question: {3}

                Provide helpful, step-by-step guidance that addresses the student's specific question and helps them understand the problem better. Be educational and supportive.
                Keep your response concise and under 500 characters.

                Return the result as raw JSON with the following structure:
                {{
                    ""guidance"": ""Detailed guidance that addresses the student's question and helps them understand the problem""
                }}

                VERY IMPORTANT: Do not use markdown formatting. Do not wrap the JSON in code blocks or ```json tags. Only return the pure JSON object without any additional text or formatting. The first character should be '{{' and the last character should be '}}'.";

        public const string FallbackGuidanceMessage = "I recommend reviewing the problem step-by-step. Break it down into smaller parts and solve each part separately. Check your calculations carefully and make sure you understand the concepts involved.";

        public const string GuidanceJsonRegexPattern = @"\{.*\""guidance\"".*:.*\""(.*)\"".*\}";
        public const string GuidanceTextRegexPattern = @"guidance.*?:.*?[""'](.+?)[""']";
    }
}
