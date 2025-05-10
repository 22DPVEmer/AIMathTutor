using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Service responsible for evaluating math problem answers
    /// </summary>
    public class AnswerEvaluationService : IAnswerEvaluationService
    {
        private readonly IKernelProvider _kernelProvider;
        private readonly ILogger<AnswerEvaluationService> _logger;
        private readonly IJsonService _jsonService;

        /// <summary>
        /// Initializes a new instance of the AnswerEvaluationService class
        /// </summary>
        /// <param name="kernelProvider">The kernel provider</param>
        /// <param name="jsonService">The JSON service</param>
        /// <param name="logger">The logger</param>
        public AnswerEvaluationService(
            IKernelProvider kernelProvider,
            IJsonService jsonService,
            ILogger<AnswerEvaluationService> logger)
        {
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));
            _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Evaluates if a user's answer to a math problem is correct
        /// </summary>
        /// <param name="problem">The math problem statement</param>
        /// <param name="userAnswer">The user's answer</param>
        /// <returns>JSON string with evaluation result and feedback</returns>
        public async Task<string> EvaluateAnswerAsync(string problem, string userAnswer)
        {
            try
            {
                _logger.LogInformation("Evaluating answer for problem: {Problem}", problem);

                string prompt = $@"Evaluate if the following answer to the math problem is correct. Be helpful and educational, but also be forgiving and generous in your evaluation.

                Problem: {problem}
                User's Answer: {userAnswer}

                Return the result as raw JSON with the following structure:
                {{
                    ""isCorrect"": true/false,
                    ""feedback"": ""Detailed feedback on the answer""
                }}

                Keep the feedback brief and under 200 characters. If the answer is partially correct, consider marking it as correct and provide guidance in the feedback.

                VERY IMPORTANT: Do not use markdown formatting. Do not wrap the JSON in code blocks or ```json tags. Only return the pure JSON object without any additional text or formatting. The first character should be '{{' and the last character should be '}}'.";

                var response = await _kernelProvider.InvokePromptAsync(prompt, 0.3, 500);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogWarning("Empty response received from AI model for evaluation");
                    return CreateFallbackEvaluation(false);
                }

                // Validate JSON format
                if (!_jsonService.IsValidJson(response))
                {
                    _logger.LogWarning("Invalid JSON received from AI model for evaluation");
                    return CreateFallbackEvaluation(false);
                }

                // Check for required properties
                if (!_jsonService.HasRequiredProperties(response, "isCorrect", "feedback"))
                {
                    _logger.LogWarning("Missing required properties in JSON response for evaluation");
                    return CreateFallbackEvaluation(false);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating answer: {Message}", ex.Message);
                return CreateFallbackEvaluation(false);
            }
        }

        private string CreateFallbackEvaluation(bool isCorrect)
        {
            string feedback = isCorrect
                ? "Your answer appears to be correct."
                : "Your answer appears to be incorrect. Please check your work and try again.";

            return $@"{{
                ""isCorrect"": {isCorrect.ToString().ToLower()},
                ""feedback"": ""{_jsonService.EscapeJsonString(feedback)}""
            }}";
        }
    }
}
