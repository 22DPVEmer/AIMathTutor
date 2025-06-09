using MathTutor.Application.Constants;
using MathTutor.Application.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProblemGenerationService> _logger;

        /// <summary>
        /// Initializes a new instance of the ProblemGenerationService class
        /// </summary>
        /// <param name="kernelProvider">The kernel provider</param>
        /// <param name="jsonService">The JSON service</param>
        /// <param name="logger">The logger instance</param>
        public ProblemGenerationService(
            IKernelProvider kernelProvider,
            IJsonService jsonService,
            ILogger<ProblemGenerationService> logger)
        {
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));
            _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogInformation("Starting problem generation for topic: {Topic}, difficulty: {Difficulty}", topic, difficulty);

                string prompt = string.Format(ProblemGenerationServiceConstants.ProblemGenerationPromptTemplate, topic, difficulty);
                _logger.LogDebug("Generated prompt: {Prompt}", prompt);

                var response = await _kernelProvider.InvokePromptAsync(
                    prompt,
                    ProblemGenerationServiceConstants.ModelParameters.Temperature,
                    ProblemGenerationServiceConstants.ModelParameters.MaxTokens);

                _logger.LogDebug("AI Response received: {Response}", response);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogWarning("AI returned empty or null response for topic: {Topic}", topic);
                    return CreateFallbackProblem(topic);
                }

                // Extract JSON from markdown code blocks if present
                string cleanedResponse = ExtractJsonFromResponse(response);
                _logger.LogDebug("Cleaned response: {CleanedResponse}", cleanedResponse);

                // Validate JSON format
                if (!_jsonService.IsValidJson(cleanedResponse))
                {
                    _logger.LogWarning("AI returned invalid JSON for topic: {Topic}. Cleaned Response: {CleanedResponse}", topic, cleanedResponse);
                    return CreateFallbackProblem(topic);
                }

                // Check for required properties
                if (!_jsonService.HasRequiredProperties(cleanedResponse, ProblemGenerationServiceConstants.RequiredProperties.Statement))
                {
                    _logger.LogWarning("AI response missing required 'statement' property for topic: {Topic}. Response: {CleanedResponse}", topic, cleanedResponse);
                    return CreateFallbackProblem(topic);
                }

                // If solution or explanation is missing, create a structured response
                if (!_jsonService.HasRequiredProperties(cleanedResponse,
                    ProblemGenerationServiceConstants.RequiredProperties.Solution,
                    ProblemGenerationServiceConstants.RequiredProperties.Explanation))
                {
                    _logger.LogInformation("AI response missing solution or explanation, creating structured response for topic: {Topic}", topic);
                    return _jsonService.CreateStructuredResponse(cleanedResponse);
                }

                _logger.LogInformation("Successfully generated problem for topic: {Topic}", topic);
                return cleanedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while generating problem for topic: {Topic}, difficulty: {Difficulty}", topic, difficulty);
                return CreateFallbackProblem(topic);
            }
        }

        private string CreateFallbackProblem(string topic)
        {
            return string.Format(ProblemGenerationServiceConstants.FallbackProblemTemplate, topic);
        }

        /// <summary>
        /// Extracts JSON from a response that may be wrapped in markdown code blocks
        /// </summary>
        /// <param name="response">The raw response from AI</param>
        /// <returns>Clean JSON string</returns>
        private string ExtractJsonFromResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return response;

            // Remove markdown code block formatting
            var cleaned = response.Trim();

            // Remove ```json at the beginning
            if (cleaned.StartsWith("```json"))
            {
                cleaned = cleaned.Substring(7).Trim();
            }
            else if (cleaned.StartsWith("```"))
            {
                cleaned = cleaned.Substring(3).Trim();
            }

            // Remove ``` at the end
            if (cleaned.EndsWith("```"))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 3).Trim();
            }

            return cleaned;
        }
    }
}
