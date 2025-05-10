using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using MathTutor.Application.Plugins;
using System;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Service for mathematical operations using Semantic Kernel
    /// </summary>
    public sealed class MathKernelService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<MathKernelService> _logger;
        private readonly KernelPlugin _mathPlugin;

        /// <summary>
        /// Initializes a new instance of the MathKernelService class
        /// </summary>
        /// <param name="kernel">The Semantic Kernel instance</param>
        /// <param name="logger">The logger</param>
        public MathKernelService(Kernel kernel, ILogger<MathKernelService> logger)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Check if the plugin is already registered
            try
            {
                if (_kernel.Plugins.TryGetPlugin("MathPlugin", out var existingPlugin))
                {
                    _mathPlugin = existingPlugin;
                    _logger.LogDebug("Using existing MathPlugin instance");
                }
                else
                {
                    _mathPlugin = kernel.ImportPluginFromObject(new MathPlugin(), "MathPlugin");
                    _logger.LogDebug("Created new MathPlugin instance");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing MathPlugin");
                throw;
            }
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
                var function = _kernel.CreateFunctionFromPrompt(
                    @"Generate a {{$difficulty}} math problem about {{$topic}}.
                    Respond ONLY with valid JSON using this structure:
                    {
                        ""statement"": ""problem statement"",
                        ""solution"": ""correct solution"",
                        ""explanation"": ""step-by-step explanation""
                    }");

                var result = await _kernel.InvokeAsync(function, new() {
                    ["difficulty"] = difficulty,
                    ["topic"] = topic
                });

                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating math problem");
                throw;
            }
        }

        /// <summary>
        /// Validates if a mathematical expression is syntactically correct
        /// </summary>
        /// <param name="expression">The mathematical expression to validate</param>
        /// <returns>True if the expression is valid, false otherwise</returns>
        public async Task<bool> ValidateMathExpressionAsync(string expression)
        {
            try
            {
                return await _kernel.InvokeAsync<bool>(
                    _mathPlugin["ValidateExpression"],
                    new() { ["expression"] = expression }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating expression");
                throw;
            }
        }

        /// <summary>
        /// Checks if two mathematical expressions are algebraically equivalent
        /// </summary>
        /// <param name="expr1">First expression</param>
        /// <param name="expr2">Second expression</param>
        /// <returns>True if the expressions are equivalent, false otherwise</returns>
        public async Task<bool> CheckExpressionEquivalenceAsync(string expr1, string expr2)
        {
            try
            {
                return await _kernel.InvokeAsync<bool>(
                    _mathPlugin["AreEquivalent"],
                    new() {
                        ["expr1"] = expr1,
                        ["expr2"] = expr2
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking equivalence");
                throw;
            }
        }

        /// <summary>
        /// Simplifies a mathematical expression
        /// </summary>
        /// <param name="expression">Expression to simplify</param>
        /// <returns>The simplified expression</returns>
        public async Task<string> SimplifyExpressionAsync(string expression)
        {
            try
            {
                var result = await _kernel.InvokeAsync<string>(
                    _mathPlugin["SimplifyExpression"],
                    new() { ["expression"] = expression }
                );
                return result ?? expression;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error simplifying expression");
                throw;
            }
        }

        /// <summary>
        /// Evaluates a mathematical expression numerically
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>The numerical result</returns>
        public async Task<double> EvaluateExpressionAsync(string expression)
        {
            try
            {
                return await _kernel.InvokeAsync<double>(
                    _mathPlugin["EvaluateExpression"],
                    new() { ["expression"] = expression }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating expression");
                throw;
            }
        }
    }


}