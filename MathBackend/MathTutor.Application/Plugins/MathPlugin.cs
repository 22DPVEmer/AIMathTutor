using Microsoft.SemanticKernel;
using MathNet.Symbolics;
using System.ComponentModel;

namespace MathTutor.Application.Plugins
{
    /// <summary>
    /// Plugin for mathematical operations
    /// </summary>
    public sealed class MathPlugin
    {
        /// <summary>
        /// Validates if a mathematical expression is syntactically correct
        /// </summary>
        /// <param name="expression">The mathematical expression to validate</param>
        /// <returns>True if the expression is valid, false otherwise</returns>
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

        /// <summary>
        /// Checks if two mathematical expressions are algebraically equivalent
        /// </summary>
        /// <param name="expr1">First expression</param>
        /// <param name="expr2">Second expression</param>
        /// <returns>True if the expressions are equivalent, false otherwise</returns>
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

        /// <summary>
        /// Simplifies a mathematical expression
        /// </summary>
        /// <param name="expression">Expression to simplify</param>
        /// <returns>The simplified expression</returns>
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

        /// <summary>
        /// Evaluates a mathematical expression numerically
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>The numerical result</returns>
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
