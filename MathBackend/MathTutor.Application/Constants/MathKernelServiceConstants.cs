namespace MathTutor.Application.Constants
{
    /// <summary>
    /// Constants used in the MathKernelService
    /// </summary>
    public static class MathKernelServiceConstants
    {
        // Plugin names
        public const string MathPluginName = "MathPlugin";

        // Function names
        public const string ValidateExpressionFunction = "ValidateExpression";
        public const string AreEquivalentFunction = "AreEquivalent";
        public const string SimplifyExpressionFunction = "SimplifyExpression";
        public const string EvaluateExpressionFunction = "EvaluateExpression";

        // Parameter names
        public const string ExpressionParameter = "expression";
        public const string Expr1Parameter = "expr1";
        public const string Expr2Parameter = "expr2";
        public const string DifficultyParameter = "difficulty";
        public const string TopicParameter = "topic";

        // Log messages
        public const string UsingExistingPluginMessage = "Using existing MathPlugin instance";
        public const string CreatedNewPluginMessage = "Created new MathPlugin instance";
        public const string PluginInitializationErrorMessage = "Error initializing MathPlugin";
        public const string GenerationErrorMessage = "Error generating math problem";
        public const string ValidationErrorMessage = "Error validating expression";
        public const string EquivalenceErrorMessage = "Error checking equivalence";
        public const string SimplificationErrorMessage = "Error simplifying expression";
        public const string EvaluationErrorMessage = "Error evaluating expression";

        // Prompt templates
        public const string GenerateMathProblemPrompt = @"Generate a {{$difficulty}} math problem about {{$topic}}.
                    Respond ONLY with valid JSON using this structure:
                    {
                        ""statement"": ""problem statement"",
                        ""solution"": ""correct solution"",
                        ""explanation"": ""step-by-step explanation""
                    }";
    }
}
