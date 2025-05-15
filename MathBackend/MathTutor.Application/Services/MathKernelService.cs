using Microsoft.SemanticKernel;
using MathTutor.Application.Constants;
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
        private readonly KernelPlugin _mathPlugin;

        /// <summary>
        /// Initializes a new instance of the MathKernelService class
        /// </summary>
        /// <param name="kernel">The Semantic Kernel instance</param>
        public MathKernelService(Kernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));

            // Check if the plugin is already registered
            try
            {
                if (_kernel.Plugins.TryGetPlugin(MathKernelServiceConstants.MathPluginName, out var existingPlugin))
                {
                    _mathPlugin = existingPlugin;
                }
                else
                {
                    _mathPlugin = kernel.ImportPluginFromObject(new MathPlugin(), MathKernelServiceConstants.MathPluginName);
                }
            }
            catch (Exception)
            {
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
                var function = _kernel.CreateFunctionFromPrompt(MathKernelServiceConstants.GenerateMathProblemPrompt);

                var result = await _kernel.InvokeAsync(function, new() {
                    [MathKernelServiceConstants.DifficultyParameter] = difficulty,
                    [MathKernelServiceConstants.TopicParameter] = topic
                });

                return result.ToString();
            }
            catch (Exception)
            {
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
                    _mathPlugin[MathKernelServiceConstants.ValidateExpressionFunction],
                    new() { [MathKernelServiceConstants.ExpressionParameter] = expression }
                );
            }
            catch (Exception)
            {
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
                    _mathPlugin[MathKernelServiceConstants.AreEquivalentFunction],
                    new() {
                        [MathKernelServiceConstants.Expr1Parameter] = expr1,
                        [MathKernelServiceConstants.Expr2Parameter] = expr2
                    }
                );
            }
            catch (Exception)
            {
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
                    _mathPlugin[MathKernelServiceConstants.SimplifyExpressionFunction],
                    new() { [MathKernelServiceConstants.ExpressionParameter] = expression }
                );
                return result ?? expression;
            }
            catch (Exception)
            {
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
                    _mathPlugin[MathKernelServiceConstants.EvaluateExpressionFunction],
                    new() { [MathKernelServiceConstants.ExpressionParameter] = expression }
                );
            }
            catch (Exception)
            {
                throw;
            }
        }
    }


}