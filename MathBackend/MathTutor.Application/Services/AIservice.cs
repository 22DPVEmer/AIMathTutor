using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace MathTutor.Application.Services
{
    public class AIservice : IAIservice
    {
        private readonly Kernel _kernel;
        private readonly ILogger<AIservice> _logger;

        public AIservice(IConfiguration configuration, ILogger<AIservice> logger)
        {
            _logger = logger;
            
            try
            {
                string apiKey = configuration["AI:Gemini:ApiKey"] ?? 
                    throw new InvalidOperationException("Gemini API Key not found in configuration");

                // Configure Semantic Kernel with Gemini
#pragma warning disable SKEXP0070 
                _kernel = Kernel.CreateBuilder()
    .AddGoogleAIGeminiChatCompletion("gemini-pro", apiKey)
    .Build();
#pragma warning restore SKEXP0070

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Semantic Kernel with Gemini");
                throw;
            }
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                var result = await _kernel.InvokePromptAsync(prompt);
                return result.GetValue<string>() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI response");
                return "I'm sorry, I couldn't generate a response at this time.";
            }
        }

        public async Task<string> GenerateMathProblemAsync(string topic, string difficulty)
        {
            try
            {
                string prompt = $@"Generate a math problem on the topic of {topic} with {difficulty} difficulty level.
                The response should be in the following JSON format:
                {{
                    ""statement"": ""The problem statement"",
                    ""solution"": ""The correct answer"",
                    ""explanation"": ""Step-by-step explanation of how to solve the problem""
                }}

                Important: Only return valid JSON in the exact format specified above. Do not add any markdown formatting or code blocks. Do not include any additional text before or after the JSON.";

                var result = await _kernel.InvokePromptAsync(prompt, new KernelArguments
                {
                    { "Temperature", 0.7 }
                });
                
                return result.GetValue<string>() ?? "{}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating math problem: {Message}", ex.Message);
                return "{}";
            }
        }

        public async Task<string> EvaluateAnswerAsync(string problem, string userAnswer)
        {
            try
            {
                string prompt = $@"Evaluate if the following answer to the math problem is correct. Be helpful and educational.
                Problem: {problem}
                User's Answer: {userAnswer}
                
                Respond with a JSON in the following format:
                {{
                    ""isCorrect"": true/false,
                    ""feedback"": ""Detailed feedback explaining whether the answer is correct or not and why""
                }}

                Important: Only return valid JSON in the exact format specified above. Do not add any markdown formatting or code blocks. Do not include any additional text before or after the JSON.";

                var result = await _kernel.InvokePromptAsync(prompt, new KernelArguments
                {
                    { "Temperature", 0.3 }
                });
                
                return result.GetValue<string>() ?? "{}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating answer: {Message}", ex.Message);
                return "{}";
            }
        }
    }
}