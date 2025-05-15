using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    /// <summary>
    /// Service responsible for generating math problems
    /// </summary>
    public interface IProblemGenerationService
    {
        /// <summary>
        /// Generates a math problem on a specific topic with specified difficulty
        /// </summary>
        /// <param name="topic">The math topic</param>
        /// <param name="difficulty">The difficulty level</param>
        /// <returns>JSON string with problem statement, solution, and explanation</returns>
        Task<string> GenerateMathProblemAsync(string topic, string difficulty);
    }
}
