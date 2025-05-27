using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    /// <summary>
    /// Interface for mathematical operations using Semantic Kernel
    /// </summary>
    public interface IMathKernelService
    {
        /// <summary>
        /// Generates a math problem on a specific topic with specified difficulty
        /// </summary>
        /// <param name="topic">The math topic</param>
        /// <param name="difficulty">The difficulty level</param>
        /// <returns>JSON string with problem statement, solution, and explanation</returns>
        Task<string> GenerateMathProblemAsync(string topic, string difficulty);

        /// <summary>
        /// Validates if a mathematical expression is syntactically correct
        /// </summary>
        /// <param name="expression">The mathematical expression to validate</param>
        /// <returns>True if the expression is valid, false otherwise</returns>
        Task<bool> ValidateMathExpressionAsync(string expression);

        /// <summary>
        /// Checks if two mathematical expressions are algebraically equivalent
        /// </summary>
        /// <param name="expr1">First expression</param>
        /// <param name="expr2">Second expression</param>
        /// <returns>True if the expressions are equivalent, false otherwise</returns>
        Task<bool> CheckExpressionEquivalenceAsync(string expr1, string expr2);

        /// <summary>
        /// Simplifies a mathematical expression
        /// </summary>
        /// <param name="expression">Expression to simplify</param>
        /// <returns>The simplified expression</returns>
        Task<string> SimplifyExpressionAsync(string expression);

        /// <summary>
        /// Evaluates a mathematical expression numerically
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>The numerical result</returns>
        Task<double> EvaluateExpressionAsync(string expression);
    }
}
