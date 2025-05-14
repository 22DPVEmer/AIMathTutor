using MathTutor.Application.Constants;
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
        private readonly IJsonService _jsonService;

        /// <summary>
        /// Initializes a new instance of the GuidanceService class
        /// </summary>
        /// <param name="kernelProvider">The kernel provider</param>
        /// <param name="jsonService">The JSON service</param>
        public GuidanceService(
            IKernelProvider kernelProvider,
            IJsonService jsonService)
        {
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));
            _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
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
                string prompt = string.Format(
                    GuidanceServiceConstants.GuidancePromptTemplate,
                    problem,
                    solution,
                    userAnswer,
                    question);

                var response = await _kernelProvider.InvokePromptAsync(
                    prompt,
                    GuidanceServiceConstants.GuidanceTemperature,
                    GuidanceServiceConstants.GuidanceMaxTokens);

                if (string.IsNullOrWhiteSpace(response))
                {
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
                    return CreateFallbackGuidance();
                }

                // Check for required properties
                if (!_jsonService.HasRequiredProperties(response, "guidance"))
                {
                    return CreateFallbackGuidance();
                }

                return response;
            }
            catch (Exception)
            {
                return CreateFallbackGuidance();
            }
        }

        private string ExtractGuidanceFromResponse(string response)
        {
            try
            {
                // Try to extract JSON from the response
                var match = Regex.Match(response, GuidanceServiceConstants.GuidanceJsonRegexPattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }

                // If no JSON found, try to extract any text that might be useful
                match = Regex.Match(response, GuidanceServiceConstants.GuidanceTextRegexPattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }

                // If still no match, just return the response as is (up to max length)
                if (response.Length > GuidanceServiceConstants.MaxGuidanceLength)
                {
                    return response.Substring(0, GuidanceServiceConstants.MaxGuidanceLength);
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
                guidance = GuidanceServiceConstants.FallbackGuidanceMessage
            };
            return _jsonService.Serialize(fallbackObj);
        }
    }
}
