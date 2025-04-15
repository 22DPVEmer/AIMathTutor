using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;

namespace MathTutor.Application.Services
{
    public class AIservice : IAIservice
    {
        private readonly Kernel _kernel;
        private readonly ILogger<AIservice> _logger;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public AIservice(
            Kernel kernel,
            IConfiguration configuration, 
            ILogger<AIservice> logger, 
            HttpClient httpClient)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            try
            {
                _apiKey = configuration["AI:Gemini:ApiKey"] ?? 
                    throw new InvalidOperationException("Gemini API Key not found in configuration");
                
                _logger.LogInformation("AIService initialized with Semantic Kernel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing AIService: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                _logger.LogDebug("Sending prompt to Gemini API: {Prompt}", prompt);
                var result = await _kernel.InvokePromptAsync(prompt);
                var response = result.GetValue<string>() ?? string.Empty;
                _logger.LogDebug("Received response from Gemini API: {Response}", response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI response: {Message}", ex.Message);
                return "I'm sorry, I couldn't generate a response at this time.";
            }
        }

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

                _logger.LogDebug("Sending prompt to Gemini API: {Prompt}", prompt);

                var result = await _kernel.InvokePromptAsync(prompt, new KernelArguments
                {
                    { "Temperature", 0.7 },
                    { "MaxTokens", 800 }
                });
                
                var response = result.GetValue<string>();
                _logger.LogDebug("Received response from Gemini API: {Response}", response);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogWarning("Empty response received from Gemini API");
                    return "{}";
                }

                // Clean response by removing markdown code blocks
                response = CleanJsonResponse(response);
                
                // Handle potentially truncated JSON by ensuring it ends with }
                if (!response.TrimEnd().EndsWith("}"))
                {
                    _logger.LogWarning("Response appears to be truncated. Attempting to fix.");
                    response = TryFixTruncatedJson(response);
                }

                // Validate JSON format
                try
                {
                    var jsonObject = JsonSerializer.Deserialize<JsonElement>(response);
                    if (!jsonObject.TryGetProperty("statement", out _))
                    {
                        _logger.LogWarning("Missing 'statement' property in JSON response");
                        return CreateFallbackProblem(topic);
                    }
                    
                    // If solution or explanation is missing, we can still use the problem
                    if (!jsonObject.TryGetProperty("solution", out _) || 
                        !jsonObject.TryGetProperty("explanation", out _))
                    {
                        _logger.LogWarning("Some properties missing in JSON. Creating structured response.");
                        return CreateStructuredResponse(jsonObject);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Invalid JSON received from Gemini API");
                    return CreateFallbackProblem(topic);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating math problem: {Message}", ex.Message);
                return CreateFallbackProblem(topic);
            }
        }

        public async Task<string> EvaluateAnswerAsync(string problem, string userAnswer)
        {
            try
            {
                _logger.LogInformation("Evaluating answer for problem: {Problem}", problem);

                string prompt = $@"Evaluate if the following answer to the math problem is correct. Be helpful and educational.
                Problem: {problem}
                User's Answer: {userAnswer}
                
                Return the result as raw JSON with the following structure:
                {{
                    ""isCorrect"": true/false,
                    ""feedback"": ""Detailed feedback explaining whether the answer is correct or not and why""
                }}

                Keep the feedback brief and under 200 characters.
                VERY IMPORTANT: Do not use markdown formatting. Do not wrap the JSON in code blocks or ```json tags. Only return the pure JSON object without any additional text or formatting. The first character should be '{{' and the last character should be '}}'.";

                _logger.LogDebug("Sending evaluation prompt to Gemini API: {Prompt}", prompt);

                var result = await _kernel.InvokePromptAsync(prompt, new KernelArguments
                {
                    { "Temperature", 0.3 },
                    { "MaxTokens", 500 }
                });
                
                var response = result.GetValue<string>();
                _logger.LogDebug("Received evaluation response from Gemini API: {Response}", response);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogWarning("Empty response received from Gemini API for evaluation");
                    return CreateFallbackEvaluation();
                }

                // Clean response by removing markdown code blocks
                response = CleanJsonResponse(response);
                
                // Handle potentially truncated JSON by ensuring it ends with }
                if (!response.TrimEnd().EndsWith("}"))
                {
                    _logger.LogWarning("Response appears to be truncated. Attempting to fix.");
                    response = TryFixTruncatedJson(response);
                }

                // Validate JSON format
                try
                {
                    var jsonObject = JsonSerializer.Deserialize<JsonElement>(response);
                    
                    // Create structured response if any required properties are missing
                    if (!jsonObject.TryGetProperty("isCorrect", out _) ||
                        !jsonObject.TryGetProperty("feedback", out _))
                    {
                        _logger.LogWarning("Missing required properties in evaluation response");
                        return CreateStructuredEvaluation(jsonObject);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Invalid JSON received from Gemini API for evaluation");
                    return CreateFallbackEvaluation();
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating answer: {Message}", ex.Message);
                return CreateFallbackEvaluation();
            }
        }

        private string CleanJsonResponse(string response)
        {
            // Remove markdown code blocks if present
            if (response.StartsWith("```json") || response.StartsWith("```"))
            {
                response = response.Replace("```json", "").Replace("```", "").Trim();
            }
            
            // Find the first { and last } to extract valid JSON
            int startIndex = response.IndexOf('{');
            int endIndex = response.LastIndexOf('}');
            
            if (startIndex >= 0 && endIndex > startIndex)
            {
                response = response.Substring(startIndex, endIndex - startIndex + 1);
            }
            
            _logger.LogDebug("Cleaned JSON response: {Response}", response);
            
            return response;
        }

        // Additional helper methods
        private string TryFixTruncatedJson(string json)
        {
            try
            {
                // Count opening and closing braces to see if we're missing some
                int openBraces = json.Count(c => c == '{');
                int closeBraces = json.Count(c => c == '}');
                
                if (openBraces > closeBraces)
                {
                    // Add missing closing braces
                    json += new string('}', openBraces - closeBraces);
                    _logger.LogInformation("Added {Count} closing braces to fix truncated JSON", openBraces - closeBraces);
                }
                
                return json;
            }
            catch
            {
                return json; // Return original if fixing fails
            }
        }
        
        private string CreateFallbackProblem(string topic)
        {
            return $@"{{
                ""statement"": ""Simple {topic} problem: Unable to generate a custom problem at this time."",
                ""solution"": ""Contact your teacher for assistance."",
                ""explanation"": ""The system encountered an error while generating this problem.""
            }}";
        }
        
        private string CreateStructuredResponse(JsonElement partialJson)
        {
            try
            {
                string statement = "";
                string solution = "Unknown";
                string explanation = "No detailed explanation available.";
                
                if (partialJson.TryGetProperty("statement", out var statementElem))
                {
                    statement = statementElem.GetString() ?? "";
                }
                
                if (partialJson.TryGetProperty("solution", out var solutionElem))
                {
                    solution = solutionElem.GetString() ?? "Unknown";
                }
                
                if (partialJson.TryGetProperty("explanation", out var explanationElem))
                {
                    explanation = explanationElem.GetString() ?? "No detailed explanation available.";
                }
                
                return $@"{{
                    ""statement"": ""{EscapeJsonString(statement)}"",
                    ""solution"": ""{EscapeJsonString(solution)}"",
                    ""explanation"": ""{EscapeJsonString(explanation)}""
                }}";
            }
            catch
            {
                return CreateFallbackProblem("math");
            }
        }
        
        private string EscapeJsonString(string str)
        {
            return str.Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
        }
        
        private string CreateFallbackEvaluation()
        {
            return @"{
                ""isCorrect"": false,
                ""feedback"": ""Unable to evaluate your answer at this time. Please try again later or contact your instructor for assistance.""
            }";
        }
        
        private string CreateStructuredEvaluation(JsonElement partialJson)
        {
            try
            {
                bool isCorrect = false;
                string feedback = "Unable to properly evaluate your answer.";
                
                if (partialJson.TryGetProperty("isCorrect", out var correctElem))
                {
                    if (correctElem.ValueKind == JsonValueKind.True)
                    {
                        isCorrect = true;
                    }
                    else if (correctElem.ValueKind == JsonValueKind.String)
                    {
                        string val = correctElem.GetString()?.ToLower() ?? "";
                        isCorrect = val == "true";
                    }
                }
                
                if (partialJson.TryGetProperty("feedback", out var feedbackElem))
                {
                    feedback = feedbackElem.GetString() ?? feedback;
                }
                
                return $@"{{
                    ""isCorrect"": {isCorrect.ToString().ToLower()},
                    ""feedback"": ""{EscapeJsonString(feedback)}""
                }}";
            }
            catch
            {
                return CreateFallbackEvaluation();
            }
        }
    }
}