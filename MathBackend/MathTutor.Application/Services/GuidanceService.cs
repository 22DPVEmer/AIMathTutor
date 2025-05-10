using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Service responsible for providing guidance on math problems
    /// </summary>
    public class GuidanceService : IGuidanceService
    {
        private readonly IKernelProvider _kernelProvider;
        private readonly ILogger<GuidanceService> _logger;
        private readonly IJsonService _jsonService;

        /// <summary>
        /// Initializes a new instance of the GuidanceService class
        /// </summary>
        /// <param name="kernelProvider">The kernel provider</param>
        /// <param name="jsonService">The JSON service</param>
        /// <param name="logger">The logger</param>
        public GuidanceService(
            IKernelProvider kernelProvider,
            IJsonService jsonService,
            ILogger<GuidanceService> logger)
        {
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));
            _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Provides guidance for a student working on a math problem
        /// </summary>
        /// <param name="problem">The math problem statement</param>
        /// <param name="solution">The correct solution</param>
        /// <param name="userAnswer">The user's answer</param>
        /// <param name="question">The student's specific question</param>
        /// <returns>JSON string with guidance</returns>
        public async Task<string> GetGuidanceAsync(string problem, string solution, string userAnswer, string question)
        {
            try
            {
                _logger.LogInformation("Generating guidance for problem: {Problem}", problem);

                string prompt = $@"Provide guidance for a student who is working on this math problem:
                Problem: {problem}
                Correct Solution: {solution}
                Student's Answer: {userAnswer}
                Student's Question: {question}

                Provide helpful, step-by-step guidance that addresses the student's specific question and helps them understand the problem better. Be educational and supportive.
                Keep your response concise and under 500 characters.

                Return the result as raw JSON with the following structure:
                {{
                    ""guidance"": ""Detailed guidance that addresses the student's question and helps them understand the problem""
                }}

                VERY IMPORTANT: Do not use markdown formatting. Do not wrap the JSON in code blocks or ```json tags. Only return the pure JSON object without any additional text or formatting. The first character should be '{{' and the last character should be '}}'.";

                var response = await _kernelProvider.InvokePromptAsync(prompt, 0.5, 600);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogWarning("Empty response received from AI model for guidance");
                    return CreateFallbackGuidance();
                }

                // Extract any useful text from the response, even if it's not valid JSON
                string extractedGuidance = ExtractGuidanceFromResponse(response);
                if (!string.IsNullOrWhiteSpace(extractedGuidance))
                {
                    // Create a valid JSON response with the extracted guidance
                    // Use JsonSerializer to properly handle the JSON structure
                    var guidanceObj = new { guidance = extractedGuidance };
                    return _jsonService.Serialize(guidanceObj);
                }

                // Validate JSON format
                if (!_jsonService.IsValidJson(response))
                {
                    _logger.LogWarning("Invalid JSON received from AI model for guidance");
                    return CreateFallbackGuidance();
                }

                // Check for required properties
                if (!_jsonService.HasRequiredProperties(response, "guidance"))
                {
                    _logger.LogWarning("Missing 'guidance' property in JSON response");
                    return CreateFallbackGuidance();
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating guidance: {Message}", ex.Message);
                return CreateFallbackGuidance();
            }
        }

        private string ExtractGuidanceFromResponse(string response)
        {
            try
            {
                // Try to extract JSON from the response
                var match = Regex.Match(response, @"\{.*\""guidance\"".*:.*\""(.*)\"".*\}");
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }

                // If no JSON found, try to extract any text that might be useful
                match = Regex.Match(response, @"guidance.*?:.*?[""'](.+?)[""']");
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }

                // If still no match, just return the response as is (up to 500 chars)
                if (response.Length > 500)
                {
                    return response.Substring(0, 500);
                }
                return response;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string CreateFallbackGuidance()
        {
            var fallbackObj = new
            {
                guidance = "I recommend reviewing the problem step-by-step. Break it down into smaller parts and solve each part separately. Check your calculations carefully and make sure you understand the concepts involved."
            };
            return _jsonService.Serialize(fallbackObj);
        }
    }
}
