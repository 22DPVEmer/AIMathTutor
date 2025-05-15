using MathTutor.Application.Constants;
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
        private readonly IJsonService _jsonService;

        /// <summary>
        /// Initializes a new instance of the AnswerEvaluationService class
        /// </summary>
        /// <param name="kernelProvider">The kernel provider</param>
        /// <param name="jsonService">The JSON service</param>
        public AnswerEvaluationService(
            IKernelProvider kernelProvider,
            IJsonService jsonService)
        {
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));
            _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
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
                string prompt = string.Format(AnswerEvaluationConstants.AnswerEvaluationPromptTemplate, problem, userAnswer);

                var response = await _kernelProvider.InvokePromptAsync(
                    prompt,
                    AnswerEvaluationConstants.AnswerEvaluationTemperature,
                    AnswerEvaluationConstants.AnswerEvaluationMaxTokens);

                if (string.IsNullOrWhiteSpace(response))
                {
                    return CreateFallbackEvaluation(false);
                }

                // Validate JSON format
                if (!_jsonService.IsValidJson(response))
                {
                    return CreateFallbackEvaluation(false);
                }

                // Check for required properties
                if (!_jsonService.HasRequiredProperties(response, "isCorrect", "feedback"))
                {
                    return CreateFallbackEvaluation(false);
                }

                return response;
            }
            catch (Exception)
            {
                return CreateFallbackEvaluation(false);
            }
        }

        private string CreateFallbackEvaluation(bool isCorrect)
        {
            string feedback = isCorrect
                ? AnswerEvaluationConstants.CorrectAnswerFeedback
                : AnswerEvaluationConstants.IncorrectAnswerFeedback;

            return $@"{{
                ""isCorrect"": {isCorrect.ToString().ToLower()},
                ""feedback"": ""{_jsonService.EscapeJsonString(feedback)}""
            }}";
        }
    }
}
