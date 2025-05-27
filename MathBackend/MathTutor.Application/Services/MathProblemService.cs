using AutoMapper;
using MathTutor.Application.Constants;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MathTutor.Core.Models;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    public class MathProblemService : IMathProblemService
    {
        private readonly IMathProblemRepository _mathProblemRepository;
        private readonly IMathTopicRepository _mathTopicRepository;
        private readonly IMathProblemAttemptRepository _mathProblemAttemptRepository;
        private readonly IAIservice _aiService;
        private readonly IMathKernelService _mathKernelService;
        private readonly IMapper _mapper;
        private static readonly JsonSerializerOptions _jsonOptions = MathProblemServiceConstants.JsonOptions;

        public MathProblemService(
            IMathProblemRepository mathProblemRepository,
            IMathTopicRepository mathTopicRepository,
            IMathProblemAttemptRepository mathProblemAttemptRepository,
            IAIservice aiService,
            IMathKernelService mathKernelService,
            IMapper mapper)
        {
            _mathProblemRepository = mathProblemRepository ?? throw new ArgumentNullException(nameof(mathProblemRepository));
            _mathTopicRepository = mathTopicRepository ?? throw new ArgumentNullException(nameof(mathTopicRepository));
            _mathProblemAttemptRepository = mathProblemAttemptRepository ?? throw new ArgumentNullException(nameof(mathProblemAttemptRepository));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _mathKernelService = mathKernelService ?? throw new ArgumentNullException(nameof(mathKernelService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<MathProblemModel>> GetAllProblemsAsync()
        {
            var problems = await _mathProblemRepository.GetAllProblemsAsync();
            return _mapper.Map<IEnumerable<MathProblemModel>>(problems);
        }

        public async Task<MathProblemModel> GetProblemByIdAsync(int id)
        {
            var problem = await _mathProblemRepository.GetProblemByIdAsync(id);
            return _mapper.Map<MathProblemModel>(problem);
        }

        public async Task<IEnumerable<MathProblemModel>> GetProblemsByTopicAsync(int topicId)
        {
            var problems = await _mathProblemRepository.GetProblemsByTopicAsync(topicId);
            return _mapper.Map<IEnumerable<MathProblemModel>>(problems);
        }

        public async Task<IEnumerable<MathProblemModel>> GetProblemsByDifficultyAsync(DifficultyLevel difficulty)
        {
            var problems = await _mathProblemRepository.GetProblemsByDifficultyAsync(difficulty);
            return _mapper.Map<IEnumerable<MathProblemModel>>(problems);
        }

        public async Task<IEnumerable<MathProblemModel>> GetProblemsByTopicAndDifficultyAsync(int topicId, DifficultyLevel difficulty)
        {
            var problems = await _mathProblemRepository.GetProblemsByTopicAndDifficultyAsync(topicId, difficulty);
            return _mapper.Map<IEnumerable<MathProblemModel>>(problems);
        }

        public async Task<MathProblemModel> CreateProblemAsync(CreateMathProblemDto problemDto)
        {
            var problem = _mapper.Map<MathProblem>(problemDto);
            var createdProblem = await _mathProblemRepository.CreateProblemAsync(problem);
            return _mapper.Map<MathProblemModel>(createdProblem);
        }

        public async Task<bool> UpdateProblemAsync(int id, UpdateMathProblemDto problemDto)
        {
            var existingProblem = await _mathProblemRepository.GetProblemByIdAsync(id);

            if (existingProblem == null)
            {
                return false;
            }

            _mapper.Map(problemDto, existingProblem);
            return await _mathProblemRepository.UpdateProblemAsync(existingProblem);
        }

        public async Task<bool> DeleteProblemAsync(int id)
        {
            return await _mathProblemRepository.DeleteProblemAsync(id);
        }

        public async Task<GeneratedMathProblemResponseDto> GenerateMathProblemAsync(GenerateMathProblemRequestDto request)
        {
            try
            {
                string difficultyLevel = MapDifficultyToString(request.Difficulty);
                string aiResponse = await _aiService.GenerateMathProblemAsync(request.Topic, difficultyLevel);

                // Use the shared JsonSerializerOptions
                GeneratedMathProblemResponseDto generatedProblem = null;

                try
                {
                    // Try to deserialize the response
                    generatedProblem = JsonSerializer.Deserialize<GeneratedMathProblemResponseDto>(aiResponse, _jsonOptions);
                }
                catch (JsonException)
                {
                    // If direct deserialization fails, try to extract the JSON portion
                    // Gemini sometimes includes additional text around the JSON
                    var jsonStart = aiResponse.IndexOf('{');
                    var jsonEnd = aiResponse.LastIndexOf('}');

                    if (jsonStart >= 0 && jsonEnd > jsonStart)
                    {
                        var jsonPart = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);

                        try {
                            generatedProblem = JsonSerializer.Deserialize<GeneratedMathProblemResponseDto>(jsonPart, _jsonOptions);
                        }
                        catch (JsonException) {
                            throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedAIResponseValidation);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedToGenerateValidProblem);
                    }
                }

                if (generatedProblem == null)
                {
                    throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedToGenerateValidProblem);
                }

                if (string.IsNullOrWhiteSpace(generatedProblem.Statement))
                {
                    throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.MissingProblemStatement);
                }

                // If TopicId is provided and SaveToDatabase is true, store the generated problem
                if (request.TopicId > 0 && request.SaveToDatabase)
                {
                    DifficultyLevel difficulty = MapStringToDifficulty(request.Difficulty);

                    var problemToCreate = new CreateMathProblemDto
                    {
                        Name = generatedProblem.Name ?? string.Format(MathProblemServiceConstants.DefaultValues.DefaultProblemNameFormat, request.Topic),
                        Statement = generatedProblem.Statement,
                        Solution = generatedProblem.Solution,
                        Explanation = generatedProblem.Explanation,
                        Difficulty = difficulty,
                        TopicId = request.TopicId
                    };

                    await CreateProblemAsync(problemToCreate);
                }

                return generatedProblem;
            }
            catch
            {
                throw;
            }
        }

        public async Task<EvaluateMathAnswerResponseDto> EvaluateAnswerAsync(EvaluateMathAnswerRequestDto request)
        {
            try
            {
                var problem = await _mathProblemRepository.GetProblemByIdAsync(request.ProblemId);

                if (problem == null)
                {
                    throw new InvalidOperationException(string.Format(MathProblemServiceConstants.ErrorMessages.ProblemNotFound, request.ProblemId));
                }

                // Check for non-answer responses BEFORE sanitization to preserve original matching
                string lowercaseUserAnswer = request.UserAnswer?.Trim().ToLower() ?? string.Empty;

                if (MathProblemServiceConstants.NonAnswerResponses.Contains(lowercaseUserAnswer))
                {
                    return new EvaluateMathAnswerResponseDto
                    {
                        IsCorrect = false,
                        Feedback = MathProblemServiceConstants.FeedbackTemplates.NonMathematicalAnswer
                    };
                }

                // Now sanitize for mathematical validation
                string normalizedUserAnswer = NormalizeAnswer(request.UserAnswer ?? string.Empty);

                // Use MathKernelService to validate and check equivalence
                bool isValidExpression = await _mathKernelService.ValidateMathExpressionAsync(normalizedUserAnswer);
                if (!isValidExpression)
                {
                    return new EvaluateMathAnswerResponseDto
                    {
                        IsCorrect = false,
                        Feedback = MathProblemServiceConstants.FeedbackTemplates.InvalidMathExpression
                    };
                }

                bool isEquivalent = await _mathKernelService.CheckExpressionEquivalenceAsync(normalizedUserAnswer, problem.Solution);

                string feedback = isEquivalent
                    ? MathProblemServiceConstants.FeedbackTemplates.CorrectPrefix + problem.Explanation
                    : MathProblemServiceConstants.FeedbackTemplates.IncorrectPrefix + problem.Solution +
                      MathProblemServiceConstants.FeedbackTemplates.ExplanationSuffix + problem.Explanation;

                return new EvaluateMathAnswerResponseDto
                {
                    IsCorrect = isEquivalent,
                    Feedback = feedback
                };
            }
            catch
            {
                throw;
            }
        }

        private static bool ContainsMathematicalContent(string answer)
        {
            // Check for mathematical symbols, numbers, or variables
            return answer.Any(c => char.IsDigit(c) || MathProblemServiceConstants.MathematicalSymbols.Contains(c));
        }

        public string SanitizeAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return string.Empty;

            // Remove whitespace, convert to lowercase, and handle common math formatting
            string result = answer.Trim().ToLower();

            foreach (var replacement in MathProblemServiceConstants.AnswerReplacements)
            {
                result = result.Replace(replacement.Key, replacement.Value);
            }

            return result;
        }

        // Kept for backward compatibility
        private string NormalizeAnswer(string answer)
        {
            return SanitizeAnswer(answer);
        }

        private static string MapDifficultyToString(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => MathProblemServiceConstants.DifficultyMapping.Easy,
                "medium" => MathProblemServiceConstants.DifficultyMapping.Medium,
                "hard" => MathProblemServiceConstants.DifficultyMapping.Hard,
                _ => MathProblemServiceConstants.DifficultyMapping.Default
            };
        }

        private static DifficultyLevel MapStringToDifficulty(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => DifficultyLevel.Easy,
                "medium" => DifficultyLevel.Medium,
                "hard" => DifficultyLevel.Hard,
                _ => DifficultyLevel.Medium
            };
        }

        private static int GetPointsForDifficulty(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => MathProblemServiceConstants.DifficultyPoints.Easy,
                "medium" => MathProblemServiceConstants.DifficultyPoints.Medium,
                "hard" => MathProblemServiceConstants.DifficultyPoints.Hard,
                _ => MathProblemServiceConstants.DifficultyPoints.Default
            };
        }

        public async Task<IEnumerable<MathProblemAttemptModel>> GetAttemptsByUserIdAsync(string userId)
        {
            try
            {
                var attempts = await _mathProblemAttemptRepository.GetAttemptsByUserIdAsync(userId);
                return _mapper.Map<IEnumerable<MathProblemAttemptModel>>(attempts);
            }
            catch
            {
                return new List<MathProblemAttemptModel>();
            }
        }

        public bool IsQuadraticEquation(string problem)
        {
            // Normalize the problem text using our SanitizeAnswer method
            string normalizedProblem = SanitizeAnswer(problem);

            // Check for common quadratic equation patterns
            return normalizedProblem.Contains(MathProblemServiceConstants.QuadraticPatterns.XSquared) ||
                   normalizedProblem.Contains(MathProblemServiceConstants.QuadraticPatterns.XPower2) ||
                   (normalizedProblem.Contains(MathProblemServiceConstants.QuadraticPatterns.QuadraticKeyword) &&
                    normalizedProblem.Contains(MathProblemServiceConstants.QuadraticPatterns.EquationKeyword)) ||
                   (normalizedProblem.Contains(MathProblemServiceConstants.QuadraticPatterns.SolveKeyword) &&
                    (normalizedProblem.Contains(MathProblemServiceConstants.QuadraticPatterns.XSquared) ||
                     normalizedProblem.Contains(MathProblemServiceConstants.QuadraticPatterns.XPower2)));
        }

        private EvaluateMathAnswerResponseDto? EvaluateQuadraticEquation(string problem, string userAnswer)
        {
            try
            {
                // Normalize the user answer using our SanitizeAnswer method
                string normalizedAnswer = SanitizeAnswer(userAnswer);

                var numbers = ExtractNumbersFromAnswer(normalizedAnswer);

                if (numbers.Count == 0)
                {
                    return null; // Fall back to AI evaluation
                }

                // Check if the answer contains any of the expected solutions
                if (problem.Contains(MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9) ||
                    problem.Contains(MathProblemServiceConstants.KnownQuadraticSolutions.XPower2Minus9))
                {
                    if (numbers.Contains(MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[0]) ||
                        numbers.Contains(MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[1]))
                    {
                        bool containsPositiveSolution = numbers.Contains(MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[0]);
                        bool containsNegativeSolution = numbers.Contains(MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[1]);

                        string feedback = containsPositiveSolution && containsNegativeSolution
                            ? string.Format(MathProblemServiceConstants.ResponseStrings.QuadraticBothSolutions,
                                MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[0],
                                MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[1])
                            : containsPositiveSolution
                                ? string.Format(MathProblemServiceConstants.ResponseStrings.QuadraticOneSolution,
                                    MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[0],
                                    MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[1])
                                : string.Format(MathProblemServiceConstants.ResponseStrings.QuadraticOneSolution,
                                    MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[1],
                                    MathProblemServiceConstants.KnownQuadraticSolutions.XSquaredMinus9Solutions[0]);

                        return new EvaluateMathAnswerResponseDto
                        {
                            IsCorrect = true,
                            Feedback = feedback
                        };
                    }
                }

                // Add more special cases for common quadratic equations here

                // If we couldn't determine the answer with our special logic, fall back to AI
                return null;
            }
            catch (Exception)
            {
                return null; // Fall back to AI evaluation
            }
        }

        // Extract numbers from an answer string
        private static List<int> ExtractNumbersFromAnswer(string answer)
        {
            var result = new List<int>();
            var numberStrings = System.Text.RegularExpressions.Regex.Matches(answer, MathProblemServiceConstants.RegexPatterns.ExtractNumbers);

            foreach (System.Text.RegularExpressions.Match match in numberStrings)
            {
                if (int.TryParse(match.Value, out int number))
                {
                    result.Add(number);
                }
            }

            return result;
        }

        public async Task<EvaluateAndSaveResultDto> EvaluateAndSaveAsync(EvaluateAndSaveRequestDto request, string userId)
        {
            try
            {
                // Sanitize user answer and solution by removing whitespace and normalizing
                string sanitizedUserAnswer = SanitizeAnswer(request.UserAnswer);
                string sanitizedSolution = string.IsNullOrWhiteSpace(request.Solution) ? string.Empty : SanitizeAnswer(request.Solution);

                EvaluateMathAnswerResponseDto evaluationResult;

                // If we have a solution, first try direct comparison with sanitized strings
                // This will be our ground truth for correctness
                bool isActuallyCorrect = false;
                if (!string.IsNullOrWhiteSpace(sanitizedSolution) && !string.IsNullOrWhiteSpace(sanitizedUserAnswer))
                {
                    // DIRECT COMPARISON: Compare user answer with the solution directly
                    // This is the absolute truth for correctness
                    bool isDirectMatch = string.Equals(sanitizedUserAnswer, sanitizedSolution, StringComparison.OrdinalIgnoreCase);

                    // Set correctness based on direct match only
                    isActuallyCorrect = isDirectMatch;

                    if (isDirectMatch)
                    {
                        evaluationResult = new EvaluateMathAnswerResponseDto
                        {
                            IsCorrect = true,
                            Feedback = MathProblemServiceConstants.FeedbackTemplates.CorrectPrefix +
                                (string.IsNullOrWhiteSpace(request.Explanation) ?
                                    MathProblemServiceConstants.ResponseStrings.MatchesExpectedSolution : request.Explanation)
                        };

                        // Skip other evaluation methods
                        goto SaveAttempt;
                    }

                    // Create a default evaluation result for incorrect answers
                    evaluationResult = new EvaluateMathAnswerResponseDto
                    {
                        IsCorrect = false,
                        Feedback = MathProblemServiceConstants.FeedbackTemplates.IncorrectPrefix + request.Solution + ". " +
                            (string.IsNullOrWhiteSpace(request.Explanation) ?
                                MathProblemServiceConstants.ResponseStrings.DoesNotMatchExpectedSolution : request.Explanation)
                    };

                    // Skip other evaluation methods
                    goto SaveAttempt;
                }

                // Special case handling for quadratic equations
                if (IsQuadraticEquation(request.Problem))
                {
                    var specialResult = EvaluateQuadraticEquation(request.Problem, request.UserAnswer);
                    if (specialResult != null)
                    {
                        evaluationResult = specialResult;
                    }
                    else
                    {
                        string aiResponse = await _aiService.EvaluateAnswerAsync(request.Problem, request.UserAnswer);

                        if (string.IsNullOrEmpty(aiResponse))
                        {
                            throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedAIResponse);
                        }

                        try
                        {
                            // Parse the AI response using the shared JsonSerializerOptions
                            var aiEvaluation = JsonSerializer.Deserialize<EvaluateMathAnswerResponseDto>(aiResponse, _jsonOptions)
                                ?? throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedToParseAIEvaluation);

                            // Override the AI's correctness determination with our ground truth if we have a solution
                            if (!string.IsNullOrWhiteSpace(sanitizedSolution))
                            {
                                // Keep the AI's feedback but use our correctness determination
                                evaluationResult = new EvaluateMathAnswerResponseDto
                                {
                                    IsCorrect = isActuallyCorrect,
                                    Feedback = isActuallyCorrect
                                        ? string.Format(MathProblemServiceConstants.ResponseStrings.CorrectWithAnswer, sanitizedSolution, aiEvaluation.Feedback)
                                        : string.Format(MathProblemServiceConstants.ResponseStrings.IncorrectWithAnswer, sanitizedSolution, aiEvaluation.Feedback)
                                };
                            }
                            else
                            {
                                // If we don't have a solution, trust the AI's evaluation
                                evaluationResult = aiEvaluation;
                            }
                        }
                        catch
                        {
                            throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedToParseAIEvaluation);
                        }
                    }
                }
                else
                {
                    string aiResponse = await _aiService.EvaluateAnswerAsync(request.Problem, request.UserAnswer);

                    if (string.IsNullOrEmpty(aiResponse))
                    {
                        throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedAIResponse);
                    }

                    try
                    {
                        // Parse the AI response using the shared JsonSerializerOptions
                        var aiEvaluation = JsonSerializer.Deserialize<EvaluateMathAnswerResponseDto>(aiResponse, _jsonOptions)
                            ?? throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedToParseAIEvaluation);

                        // Override the AI's correctness determination with our ground truth if we have a solution
                        if (!string.IsNullOrWhiteSpace(sanitizedSolution))
                        {
                            // Keep the AI's feedback but use our correctness determination
                            evaluationResult = new EvaluateMathAnswerResponseDto
                            {
                                IsCorrect = isActuallyCorrect,
                                Feedback = isActuallyCorrect
                                    ? string.Format(MathProblemServiceConstants.ResponseStrings.CorrectWithAnswer, sanitizedSolution, aiEvaluation.Feedback)
                                    : string.Format(MathProblemServiceConstants.ResponseStrings.IncorrectWithAnswer, sanitizedSolution, aiEvaluation.Feedback)
                            };
                        }
                        else
                        {
                            // If we don't have a solution, trust the AI's evaluation
                            evaluationResult = aiEvaluation;
                        }
                    }
                    catch
                    {
                        throw new InvalidOperationException(MathProblemServiceConstants.ErrorMessages.FailedToParseAIEvaluation);
                    }
                }

                SaveAttempt:

                // Check if the user already has a correct attempt for this problem
                bool hasExistingCorrectAttempt = false;

                if (request.TopicId.HasValue && request.TopicId.Value > 0)
                {
                    var attempts = await GetAttemptsByUserIdAsync(userId);

                    if (attempts.Any())
                    {
                        var problems = await GetProblemsByTopicAsync(request.TopicId.Value);
                        var matchingProblem = problems.FirstOrDefault(p =>
                            p.Statement.Equals(request.Problem, StringComparison.OrdinalIgnoreCase));

                        if (matchingProblem != null)
                        {
                            hasExistingCorrectAttempt = attempts.Any(a =>
                                a.ProblemId == matchingProblem.Id && a.IsCorrect);
                        }
                    }
                }

                // Create and save the attempt
                var attemptDto = new SaveProblemAttemptDto
                {
                    UserId = userId,
                    Name = request.Name,
                    Statement = request.Problem,
                    Solution = request.Solution,
                    Explanation = request.Explanation,
                    UserAnswer = request.UserAnswer, // Keep original answer for display purposes
                    IsCorrect = evaluationResult.IsCorrect,
                    Difficulty = request.Difficulty,
                    Topic = request.Topic,
                    TopicId = request.TopicId
                };

                var saveResult = await SaveProblemAttemptAsync(attemptDto);

                // Create the response
                var response = new EvaluateAndSaveResultDto
                {
                    Success = saveResult,
                    IsCorrect = evaluationResult.IsCorrect,
                    Feedback = evaluationResult.Feedback,
                    HasExistingCorrectAttempt = hasExistingCorrectAttempt
                };

                // If the save was successful and we have a topic ID, get updated problems and attempts
                if (saveResult && request.TopicId.HasValue)
                {
                    var problems = await GetProblemsByTopicAsync(request.TopicId.Value);
                    var attempts = await GetAttemptsByUserIdAsync(userId);

                    response.Problems = problems;
                    response.Attempts = attempts;
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SaveProblemAttemptAsync(SaveProblemAttemptDto attemptDto)
        {
            try
            {
                // Find or create a MathProblem entity
                MathProblem? problem = null;

                // If a topicId is provided, we'll try to find the existing problem or create a new one
                if (attemptDto.TopicId.HasValue && attemptDto.TopicId.Value > 0)
                {
                    // Check if topic exists
                    var topic = await _mathTopicRepository.GetTopicByIdAsync(attemptDto.TopicId.Value);
                    if (topic == null)
                    {
                        return false;
                    }

                    // Check if this problem already exists by matching statement and topicId
                    // This prevents duplicate problems from being created
                    var existingProblems = await _mathProblemRepository.GetProblemsByTopicAsync(attemptDto.TopicId.Value);
                    problem = existingProblems.FirstOrDefault(p =>
                        p.Statement.Equals(attemptDto.Statement, StringComparison.OrdinalIgnoreCase) &&
                        p.TopicId == attemptDto.TopicId.Value);
                }

                // If we have a problem ID, check if the user already has a correct attempt
                if (problem != null)
                {
                    var existingAttempts = await _mathProblemAttemptRepository.GetAttemptsByUserIdAndProblemIdAsync(attemptDto.UserId, problem.Id);

                    // Check if the user already has a correct attempt for this problem
                    var hasCorrectAttempt = existingAttempts.Any(a => a.IsCorrect);

                    if (hasCorrectAttempt)
                    {
                        // If the current attempt is correct, we don't need to save it
                        // If it's incorrect, we still don't save it, but we want to preserve the user's points

                        // Return true because we're successfully handling the request, even though we're not saving anything
                        return true;
                    }

                    // If the user doesn't have a correct attempt, delete any existing incorrect attempts
                    foreach (var existingAttempt in existingAttempts)
                    {
                        await _mathProblemAttemptRepository.DeleteAttemptAsync(existingAttempt.Id);
                    }
                }
                else
                {
                    var allUserAttempts = await _mathProblemAttemptRepository.GetAttemptsByUserIdAsync(attemptDto.UserId);

                    var matchingAttempts = allUserAttempts.Where(a => a.ProblemId == MathProblemServiceConstants.DefaultValues.DefaultProblemId).ToList();

                    var hasCorrectAttempt = matchingAttempts.Any(a => a.IsCorrect);

                    if (hasCorrectAttempt)
                    {
                        // If the current attempt is correct, we don't need to save it
                        // If it's incorrect, we still don't save it, but we want to preserve the user's points

                        return true;
                    }

                    foreach (var existingAttempt in matchingAttempts)
                    {
                        await _mathProblemAttemptRepository.DeleteAttemptAsync(existingAttempt.Id);
                    }
                }

                var attempt = new MathProblemAttempt
                {
                    UserId = attemptDto.UserId,
                    ProblemId = problem?.Id ?? MathProblemServiceConstants.DefaultValues.DefaultProblemId,
                    UserAnswer = attemptDto.UserAnswer,
                    IsCorrect = attemptDto.IsCorrect,
                    AttemptedAt = DateTime.UtcNow,
                    PointsEarned = attemptDto.IsCorrect ? GetPointsForDifficulty(attemptDto.Difficulty) : 0
                };

                if (problem != null)
                {
                    await _mathProblemAttemptRepository.CreateAttemptAsync(attempt);
                }
                else
                {
                    await _mathProblemAttemptRepository.CreateAttemptWithoutProblemAsync(attempt, attemptDto.Statement);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}