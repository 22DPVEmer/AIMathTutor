using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AIservice> _logger;
        private readonly IProblemGenerationService _problemGenerationService;
        private readonly IAnswerEvaluationService _answerEvaluationService;
        private readonly IGuidanceService _guidanceService;
        private readonly IKernelProvider _kernelProvider;

        /// <summary>
        /// Initializes a new instance of the AIService class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="problemGenerationService">The problem generation service</param>
        /// <param name="answerEvaluationService">The answer evaluation service</param>
        /// <param name="guidanceService">The guidance service</param>
        /// <param name="kernelProvider">The kernel provider</param>
        public AIservice(
            ILogger<AIservice> logger,
            IProblemGenerationService problemGenerationService,
            IAnswerEvaluationService answerEvaluationService,
            IGuidanceService guidanceService,
            IKernelProvider kernelProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _problemGenerationService = problemGenerationService ?? throw new ArgumentNullException(nameof(problemGenerationService));
            _answerEvaluationService = answerEvaluationService ?? throw new ArgumentNullException(nameof(answerEvaluationService));
            _guidanceService = guidanceService ?? throw new ArgumentNullException(nameof(guidanceService));
            _kernelProvider = kernelProvider ?? throw new ArgumentNullException(nameof(kernelProvider));

            _logger.LogInformation("Legacy AIService initialized with specialized services");
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
                _logger.LogDebug("Delegating prompt to KernelProvider: {Prompt}", prompt);
                return await _kernelProvider.InvokePromptAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI response: {Message}", ex.Message);
                return "I'm sorry, I couldn't generate a response at this time.";
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
                _logger.LogInformation("Delegating math problem generation to ProblemGenerationService: Topic={Topic}, Difficulty={Difficulty}",
                    topic, difficulty);
                
                return await _problemGenerationService.GenerateMathProblemAsync(topic, difficulty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating math problem: {Message}", ex.Message);
                return @"{
                    ""statement"": ""Simple math problem: Unable to generate a custom problem at this time."",
                    ""solution"": ""Contact your teacher for assistance."",
                    ""explanation"": ""The system encountered an error while generating this problem.""
                }";
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
                _logger.LogInformation("Delegating answer evaluation to AnswerEvaluationService: Problem={Problem}", problem);
                
                return await _answerEvaluationService.EvaluateAnswerAsync(problem, userAnswer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating answer: {Message}", ex.Message);
                return @"{
                    ""isCorrect"": false,
                    ""feedback"": ""Unable to evaluate your answer at this time. Please try again later or contact your instructor for assistance.""
                }";
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
                _logger.LogInformation("Delegating guidance request to GuidanceService: Problem={Problem}, Question={Question}",
                    problem, question);
                
                return await _guidanceService.GetGuidanceAsync(problem, solution, userAnswer, question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating guidance: {Message}", ex.Message);
                return @"{
                    ""guidance"": ""I recommend reviewing the problem step-by-step. Break it down into smaller parts and solve each part separately. Check your calculations carefully and make sure you understand the concepts involved.""
                }";
            }
        }
    }
}
