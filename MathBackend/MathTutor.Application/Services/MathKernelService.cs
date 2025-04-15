using Microsoft.SemanticKernel;
using MathNet.Numerics;
using MathNet.Symbolics;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.ComponentModel;

namespace MathTutor.Application.Services
{
    public sealed class MathKernelService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<MathKernelService> _logger;
        private readonly KernelPlugin _mathPlugin;

        public MathKernelService(Kernel kernel, ILogger<MathKernelService> logger)
        {
            _kernel = kernel;
            _logger = logger;
            
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
    }

    public sealed class MathPlugin
    {
        [KernelFunction]
        [Description("Validates if a mathematical expression is syntactically correct")]
        public bool ValidateExpression(
            [Description("The mathematical expression to validate")] string expression)
        {
            try
            {
                Infix.ParseOrThrow(expression);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [KernelFunction]
        [Description("Checks if two mathematical expressions are algebraically equivalent")]
        public bool AreEquivalent(
            [Description("First expression")] string expr1,
            [Description("Second expression")] string expr2)
        {
            try
            {
                var x = Infix.ParseOrThrow(expr1);
                var y = Infix.ParseOrThrow(expr2);
                return Algebraic.Expand(x).Equals(Algebraic.Expand(y));
            }
            catch
            {
                return false;
            }
        }

        [KernelFunction]
        [Description("Simplifies a mathematical expression")]
        public string SimplifyExpression(
            [Description("Expression to simplify")] string expression)
        {
            try
            {
                return Infix.Format(Algebraic.Expand(Infix.ParseOrThrow(expression)));
            }
            catch
            {
                return expression;
            }
        }

        [KernelFunction]
        [Description("Evaluates a mathematical expression numerically")]
        public double EvaluateExpression(
            [Description("Expression to evaluate")] string expression)
        {
            try
            {
                return Evaluate.Evaluate(
                    null,
                    Infix.ParseOrThrow(expression)
                ).RealValue;
            }
            catch
            {
                return double.NaN;
            }
        }
    }
}