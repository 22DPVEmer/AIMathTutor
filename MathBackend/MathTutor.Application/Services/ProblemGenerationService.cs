using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Service responsible for generating math problems
    /// </summary>
    public class ProblemGenerationService : IProblemGenerationService
    {
        private readonly IKernelProvider _kernelProvider;
        private readonly ILogger<ProblemGenerationService> _logger;
        private readonly IJsonService _jsonService;

        /// <summary>
        /// Initializes a new instance of the ProblemGenerationService class
        /// </summary>
        /// <param name="kernelProvider">The kernel provider</param>
        /// <param name="jsonService">The JSON service</param>
        /// <param name="logger">The logger</param>
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
                _logger.LogInformation("Generating math problem for topic: {Topic} with difficulty: {Difficulty}", topic, difficulty);

                string prompt = $@"Generate a math problem on the topic of {topic} with {difficulty} difficulty level.
                Return the result as raw JSON with the following structure:
                {{
                    ""statement"": ""The problem statement"",
                    ""solution"": ""The correct answer"",
                    ""explanation"": ""Step-by-step explanation of how to solve the problem""
                }}

                Keep the explanation brief and under 250 characters.
                VERY IMPORTANT: Do not use markdown formatting. Do not wrap the JSON in code blocks or ```json tags. Only return the pure JSON object without any additional text or formatting. The first character should be '{{' and the last character should be '}}'.";

                var response = await _kernelProvider.InvokePromptAsync(prompt, 0.7, 800);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogWarning("Empty response received from AI model");
                    return CreateFallbackProblem(topic);
                }

                // Validate JSON format
                if (!_jsonService.IsValidJson(response))
                {
                    _logger.LogWarning("Invalid JSON received from AI model");
                    return CreateFallbackProblem(topic);
                }

                // Check for required properties
                if (!_jsonService.HasRequiredProperties(response, "statement"))
                {
                    _logger.LogWarning("Missing 'statement' property in JSON response");
                    return CreateFallbackProblem(topic);
                }

                // If solution or explanation is missing, create a structured response
                if (!_jsonService.HasRequiredProperties(response, "solution", "explanation"))
                {
                    _logger.LogWarning("Some properties missing in JSON. Creating structured response.");
                    return _jsonService.CreateStructuredResponse(response);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating math problem: {Message}", ex.Message);
                return CreateFallbackProblem(topic);
            }
        }

        private string CreateFallbackProblem(string topic)
        {
            return $@"{{
                ""statement"": ""What is 2 + 2? (Fallback problem for {topic})"",
                ""solution"": ""4"",
                ""explanation"": ""Add the numbers 2 and 2 to get 4.""
            }}";
        }
    }
}
