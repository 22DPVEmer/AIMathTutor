using MathTutor.Application.Constants;
using MathTutor.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Legacy AIService that delegates to specialized services
    /// This class is maintained for backward compatibility
    /// </summary>
    public class AIservice : IAIservice
    {
        private readonly IProblemGenerationService _problemGenerationService;
        private readonly IAnswerEvaluationService _answerEvaluationService;
        private readonly IGuidanceService _guidanceService;
        private readonly IKernelProvider _kernelProvider;

        /// <summary>
        /// Initializes a new instance of the AIService class
        /// </summary>
        /// <param name="problemGenerationService">The problem generation service</param>
        /// <param name="answerEvaluationService">The answer evaluation service</param>
        /// <param name="guidanceService">The guidance service</param>
        /// <param name="kernelProvider">The kernel provider</param>
        public AIservice(
            IProblemGenerationService problemGenerationService,
            IAnswerEvaluationService answerEvaluationService,
            IGuidanceService guidanceService,
            IKernelProvider kernelProvider)
        {
            _problemGenerationService = problemGenerationService ?? throw new ArgumentNullException(nameof(problemGenerationService));
            _answerEvaluationService = answerEvaluationService ?? throw new ArgumentNullException(nameof(answerEvaluationService));
            _guidanceService = guidanceService ?? throw new ArgumentNullException(nameof(guidanceService));
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));
        }

        /// <summary>
        /// Generates a response to a prompt
        /// </summary>
        /// <param name="prompt">The prompt</param>
        /// <returns>The response</returns>
        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                return await _kernelProvider.InvokePromptAsync(prompt);
            }
            catch (Exception)
            {
                return AIServiceConstants.GenerateResponseErrorMessage;
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
                return await _problemGenerationService.GenerateMathProblemAsync(topic, difficulty);
            }
            catch (Exception)
            {
                return AIServiceConstants.MathProblemGenerationFallbackJson;
            }
        }

        /// <summary>
        /// Evaluates if a user's answer to a math problem is correct
        /// </summary>
        /// <param name="problem">The math problem statement</param>
        /// <param name="userAnswer">The user's answer</param>
        /// <returns>JSON string with evaluation result and feedback</returns>
        public async Task<string> EvaluateAnswerAsync(string problem, string userAnswer)
        {
            try
            {
                return await _answerEvaluationService.EvaluateAnswerAsync(problem, userAnswer);
            }
            catch (Exception)
            {
                return AIServiceConstants.AnswerEvaluationFallbackJson;
            }
        }

        /// <summary>
        /// Provides guidance for a student working on a math problem
        /// </summary>
        /// <param name="problem">The math problem statement</param>
        /// <param name="solution">The correct solution</param>
        /// <param name="userAnswer">The user's answer</param>
        /// <param name="question">The student's specific question</param>
        /// <returns>JSON string with guidance</returns>
        public async Task<string> GetGuidanceAsync(string problem, string solution, string userAnswer, string question)
        {
            try
            {
                return await _guidanceService.GetGuidanceAsync(problem, solution, userAnswer, question);
            }
            catch (Exception)
            {
                return AIServiceConstants.GuidanceFallbackJson;
            }
        }
    }
}
