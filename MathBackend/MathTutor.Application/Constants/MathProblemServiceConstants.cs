using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MathTutor.Application.Constants
{
    public static class MathProblemServiceConstants
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        public static readonly string[] NonAnswerResponses =
        {
            "i don't know",
            "idk",
            "dont know",
            "don't know",
            "no idea",
            "not sure",
            "unsure",
            "maybe",
            "probably",
            "perhaps"
        };

        public static readonly char[] MathematicalSymbols =
        {
            '+', '-', '*', '/', '^', '√', 'π', 'x', 'y', 'z', '='
        };

        public static readonly Dictionary<string, string> AnswerReplacements = new Dictionary<string, string>
        {
            { " ", "" },
            { "=", "" },
            { "≈", "" },
            { "~", "" },
            { "pi", "π" },
            { "sqrt", "√" }
        };

        public static class DifficultyMapping
        {
            public const string Easy = "Easy";
            public const string Medium = "Medium";
            public const string Hard = "Hard";
            public const string Default = Medium;
        }

        public static class DifficultyPoints
        {
            public const int Easy = 1;
            public const int Medium = 2;
            public const int Hard = 3;
            public const int Default = Easy;
        }

        public static class QuadraticPatterns
        {
            public const string XSquared = "x²";
            public const string XPower2 = "x^2";
            public const string QuadraticKeyword = "quadratic";
            public const string EquationKeyword = "equation";
            public const string SolveKeyword = "solve";
        }

        public static class KnownQuadraticSolutions
        {
            public const string XSquaredMinus9 = "x² - 9 = 0";
            public const string XPower2Minus9 = "x^2 - 9 = 0";
            public static readonly int[] XSquaredMinus9Solutions = { 3, -3 };
        }

        public static class FeedbackTemplates
        {
            public const string InvalidMathExpression = "Your answer is not a valid mathematical expression. Please check your input and try again.";
            public const string NonMathematicalAnswer = "Please provide a mathematical answer. If you're unsure, try to work through the problem step by step.";
            public const string CorrectPrefix = "Correct! ";
            public const string IncorrectPrefix = "Incorrect. The correct answer is: ";
            public const string ExplanationSuffix = ". Here's why: ";
        }

        public static class ErrorMessages
        {
            public const string FailedAIResponseValidation = "Failed to parse the generated math problem";
            public const string FailedToGenerateValidProblem = "Failed to generate a valid math problem";
            public const string MissingProblemStatement = "Generated problem is missing a statement";
            public const string ProblemNotFound = "Math problem with ID {0} not found";
            public const string FailedAIResponse = "Failed to get a valid response from the AI service";
            public const string FailedToParseAIEvaluation = "Failed to parse the AI evaluation response";
        }

        public static class ResponseStrings
        {
            public const string CorrectWithAnswer = "Correct! The answer is {0}. {1}";
            public const string IncorrectWithAnswer = "Incorrect. The correct answer is {0}. {1}";
            public const string MatchesExpectedSolution = "Your answer matches the expected solution.";
            public const string DoesNotMatchExpectedSolution = "Your answer does not match the expected solution.";
            public const string QuadraticBothSolutions = "Correct! The solutions are x = {0} and x = {1}.";
            public const string QuadraticOneSolution = "Correct! x = {0} is one of the solutions. The other solution is x = {1}.";
        }

        public static class DefaultValues
        {
            public const int DefaultProblemId = 0;
            public const string DefaultProblemNameFormat = "{0} Problem";
        }

        public static class RegexPatterns
        {
            public const string ExtractNumbers = @"-?\d+";
        }
    }
}
