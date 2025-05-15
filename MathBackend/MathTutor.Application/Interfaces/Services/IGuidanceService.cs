using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    /// <summary>
    /// Service responsible for providing guidance on math problems
    /// </summary>
    public interface IGuidanceService
    {
        /// <summary>
        /// Provides guidance for a student working on a math problem
        /// </summary>
        /// <param name="problem">The math problem statement</param>
        /// <param name="solution">The correct solution</param>
        /// <param name="userAnswer">The user's answer</param>
        /// <param name="question">The student's specific question</param>
        /// <returns>JSON string with guidance</returns>
        Task<string> GetGuidanceAsync(string problem, string solution, string userAnswer, string question);
    }
}
