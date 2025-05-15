using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    /// <summary>
    /// Service responsible for evaluating math problem answers
    /// </summary>
    public interface IAnswerEvaluationService
    {
        /// <summary>
        /// Evaluates if a user's answer to a math problem is correct
        /// </summary>
        /// <param name="problem">The math problem statement</param>
        /// <param name="userAnswer">The user's answer</param>
        /// <returns>JSON string with evaluation result and feedback</returns>
        Task<string> EvaluateAnswerAsync(string problem, string userAnswer);
    }
}
