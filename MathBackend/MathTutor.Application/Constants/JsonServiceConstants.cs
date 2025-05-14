using System.Text.Json;

namespace MathTutor.Application.Constants
{
    public static class JsonServiceConstants
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        // Property names
        public const string StatementProperty = "statement";
        public const string SolutionProperty = "solution";
        public const string ExplanationProperty = "explanation";

        // Error messages
        public const string SerializationErrorMessage = "Error serializing object to JSON";
        public const string DeserializationErrorMessage = "Error deserializing JSON to object";

        // Fallback values
        public const string InvalidProblemFormatMessage = "Invalid problem format";
        public const string NotAvailableMessage = "N/A";
        public const string SystemGenerationErrorMessage = "The system could not generate a valid problem.";

        // JSON templates
        public const string StructuredResponseTemplate = @"{{
            ""{0}"": ""{1}"",
            ""{2}"": ""{3}"",
            ""{4}"": ""{5}""
        }}";

        public const string FallbackResponseTemplate = @"{{
            ""{0}"": ""{1}"",
            ""{2}"": ""{3}"",
            ""{4}"": ""{5}""
        }}";
    }
}
