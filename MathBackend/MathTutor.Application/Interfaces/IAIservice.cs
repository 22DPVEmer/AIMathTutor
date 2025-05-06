using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IAIservice
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task<string> GenerateMathProblemAsync(string topic, string difficulty);
        Task<string> EvaluateAnswerAsync(string problem, string userAnswer);
        Task<string> GetGuidanceAsync(string problem, string solution, string userAnswer, string question);
    }
}