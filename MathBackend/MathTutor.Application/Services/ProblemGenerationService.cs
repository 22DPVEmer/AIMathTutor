using MathTutor.Application.Constants;
using MathTutor.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Service responsible for generating math problems
    /// </summary>
    public class ProblemGenerationService : IProblemGenerationService
    {
        private readonly IKernelProvider _kernelProvider;
        private readonly IJsonService _jsonService;

        /// <summary>
        /// Initializes a new instance of the ProblemGenerationService class
        /// </summary>
        /// <param name="kernelProvider">The kernel provider</param>
        /// <param name="jsonService">The JSON service</param>
        public ProblemGenerationService(
            IKernelProvider kernelProvider,
            IJsonService jsonService)
        {
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));
            _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
        }

        /// <summary>
        /// Generates a math problem on a specific topic with specified difficulty
        /// </summary>
        /// <param name="topic">The math topic</param>
        /// <param name="difficulty">The difficulty level</param>
        /// <returns>JSON string with problem statement, solution, and explanation</returns>
        public async Task<string> GenerateMathProblemAsync(string topic, string difficulty)
        {
            try
            {
                string prompt = string.Format(ProblemGenerationServiceConstants.ProblemGenerationPromptTemplate, topic, difficulty);

                var response = await _kernelProvider.InvokePromptAsync(
                    prompt,
                    ProblemGenerationServiceConstants.ModelParameters.Temperature,
                    ProblemGenerationServiceConstants.ModelParameters.MaxTokens);

                if (string.IsNullOrWhiteSpace(response))
                {
                    return CreateFallbackProblem(topic);
                }

                // Validate JSON format
                if (!_jsonService.IsValidJson(response))
                {
                    return CreateFallbackProblem(topic);
                }

                // Check for required properties
                if (!_jsonService.HasRequiredProperties(response, ProblemGenerationServiceConstants.RequiredProperties.Statement))
                {
                    return CreateFallbackProblem(topic);
                }

                // If solution or explanation is missing, create a structured response
                if (!_jsonService.HasRequiredProperties(response,
                    ProblemGenerationServiceConstants.RequiredProperties.Solution,
                    ProblemGenerationServiceConstants.RequiredProperties.Explanation))
                {
                    return _jsonService.CreateStructuredResponse(response);
                }

                return response;
            }
            catch (Exception)
            {
                return CreateFallbackProblem(topic);
            }
        }

        private string CreateFallbackProblem(string topic)
        {
            return string.Format(ProblemGenerationServiceConstants.FallbackProblemTemplate, topic);
        }
    }
}
