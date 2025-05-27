using AutoMapper;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Application.Services;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using MathTutor.Core.Models;
using Moq;
using System.Text.Json;
using Xunit;

namespace MathTutor.Tests.Services
{
    public class MathProblemServiceTests
    {
        private readonly Mock<IMathProblemRepository> _mockProblemRepo;
        private readonly Mock<IMathTopicRepository> _mockTopicRepo;
        private readonly Mock<IMathProblemAttemptRepository> _mockAttemptRepo;
        private readonly Mock<IAIservice> _mockAiService;
        private readonly Mock<IMathKernelService> _mockMathKernelService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MathProblemService _service;

        public MathProblemServiceTests()
        {
            _mockProblemRepo = new Mock<IMathProblemRepository>();
            _mockTopicRepo = new Mock<IMathTopicRepository>();
            _mockAttemptRepo = new Mock<IMathProblemAttemptRepository>();
            _mockAiService = new Mock<IAIservice>();
            _mockMathKernelService = new Mock<IMathKernelService>();
            _mockMapper = new Mock<IMapper>();

            _service = new MathProblemService(
                _mockProblemRepo.Object,
                _mockTopicRepo.Object,
                _mockAttemptRepo.Object,
                _mockAiService.Object,
                _mockMathKernelService.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetProblemByIdAsync_ReturnsCorrectProblem()
        {
            // Arrange
            int testId = 1;
            var testProblem = new MathProblem { Id = testId, Name = "Test Problem" };
            var testProblemModel = new MathProblemModel { Id = testId, Name = "Test Problem" };

            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(testId))
                .ReturnsAsync(testProblem);

            _mockMapper.Setup(mapper => mapper.Map<MathProblemModel>(testProblem))
                .Returns(testProblemModel);

            // Act
            var result = await _service.GetProblemByIdAsync(testId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testId, result.Id);
            Assert.Equal("Test Problem", result.Name);
            _mockProblemRepo.Verify(repo => repo.GetProblemByIdAsync(testId), Times.Once);
        }

        [Fact]
        public async Task EvaluateAnswerAsync_WithCorrectAnswer_ReturnsCorrectEvaluation()
        {
            // Arrange
            int problemId = 1;
            string userAnswer = "x = 5";
            string solution = "x = 5";
            string explanation = "Solve for x by isolating the variable";

            var request = new EvaluateMathAnswerRequestDto
            {
                ProblemId = problemId,
                UserAnswer = userAnswer
            };

            var problem = new MathProblem
            {
                Id = problemId,
                Solution = solution,
                Explanation = explanation
            };

            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(problemId))
                .ReturnsAsync(problem);

            _mockMathKernelService.Setup(service => service.ValidateMathExpressionAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockMathKernelService.Setup(service => service.CheckExpressionEquivalenceAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.EvaluateAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCorrect);
            Assert.Contains(explanation, result.Feedback);
        }

        [Fact]
        public async Task GenerateMathProblemAsync_SavesToDatabase_WhenRequested()
        {
            // Arrange
            var request = new GenerateMathProblemRequestDto
            {
                Topic = "Algebra",
                Difficulty = "Medium",
                TopicId = 1,
                SaveToDatabase = true
            };

            var aiResponse = JsonSerializer.Serialize(new GeneratedMathProblemResponseDto
            {
                Name = "Quadratic Equation",
                Statement = "Solve for x: x² - 9 = 0",
                Solution = "x = 3 or x = -3",
                Explanation = "Take the square root of both sides."
            });

            _mockAiService.Setup(service => service.GenerateMathProblemAsync(request.Topic, "Medium"))
                .ReturnsAsync(aiResponse);

            _mockProblemRepo.Setup(repo => repo.CreateProblemAsync(It.IsAny<MathProblem>()))
                .ReturnsAsync(new MathProblem { Id = 1 });

            _mockMapper.Setup(mapper => mapper.Map<MathProblemModel>(It.IsAny<MathProblem>()))
                .Returns(new MathProblemModel { Id = 1 });

            // Act
            var result = await _service.GenerateMathProblemAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Quadratic Equation", result.Name);
            Assert.Equal("Solve for x: x² - 9 = 0", result.Statement);
            _mockProblemRepo.Verify(repo => repo.CreateProblemAsync(It.IsAny<MathProblem>()), Times.Once);
        }

        // HIGH PRIORITY TESTS - Null/Not Found Scenarios
        [Fact]
        public async Task GetProblemByIdAsync_WhenProblemNotFound_ReturnsNull()
        {
            // Arrange
            int nonExistentId = 999;
            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(nonExistentId))
                .ReturnsAsync((MathProblem)null);
            _mockMapper.Setup(mapper => mapper.Map<MathProblemModel>((MathProblem)null))
                .Returns((MathProblemModel)null);

            // Act
            var result = await _service.GetProblemByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task EvaluateAnswerAsync_WhenProblemNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new EvaluateMathAnswerRequestDto
            {
                ProblemId = 999,
                UserAnswer = "x = 5"
            };

            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(999))
                .ReturnsAsync((MathProblem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.EvaluateAnswerAsync(request));

            Assert.Contains("Math problem with ID 999 not found", exception.Message);
        }

        // HIGH PRIORITY TESTS - JSON Parsing Edge Cases
        [Fact]
        public async Task GenerateMathProblemAsync_WhenAIReturnsInvalidJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new GenerateMathProblemRequestDto
            {
                Topic = "Algebra",
                Difficulty = "Medium",
                TopicId = 1,
                SaveToDatabase = false
            };

            _mockAiService.Setup(service => service.GenerateMathProblemAsync(request.Topic, "Medium"))
                .ReturnsAsync("This is not valid JSON at all");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GenerateMathProblemAsync(request));

            Assert.Contains("Failed to generate a valid math problem", exception.Message);
        }

        [Fact]
        public async Task GenerateMathProblemAsync_WhenAIReturnsEmptyStatement_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new GenerateMathProblemRequestDto
            {
                Topic = "Algebra",
                Difficulty = "Medium",
                TopicId = 1,
                SaveToDatabase = false
            };

            var aiResponse = JsonSerializer.Serialize(new GeneratedMathProblemResponseDto
            {
                Name = "Test Problem",
                Statement = "", // Empty statement
                Solution = "x = 5",
                Explanation = "Test explanation"
            });

            _mockAiService.Setup(service => service.GenerateMathProblemAsync(request.Topic, "Medium"))
                .ReturnsAsync(aiResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GenerateMathProblemAsync(request));

            Assert.Contains("Generated problem is missing a statement", exception.Message);
        }

        [Fact]
        public async Task GenerateMathProblemAsync_WhenAIReturnsJsonWithExtraText_ExtractsJsonSuccessfully()
        {
            // Arrange
            var request = new GenerateMathProblemRequestDto
            {
                Topic = "Algebra",
                Difficulty = "Medium",
                TopicId = 1,
                SaveToDatabase = false
            };

            // Simulate Gemini response with extra text around JSON
            var validJson = JsonSerializer.Serialize(new GeneratedMathProblemResponseDto
            {
                Name = "Linear Equation",
                Statement = "Solve for x: 2x + 3 = 7",
                Solution = "x = 2",
                Explanation = "Subtract 3, then divide by 2"
            });
            var aiResponseWithExtraText = $"Here's your math problem: {validJson} Hope this helps!";

            _mockAiService.Setup(service => service.GenerateMathProblemAsync(request.Topic, "Medium"))
                .ReturnsAsync(aiResponseWithExtraText);

            // Act
            var result = await _service.GenerateMathProblemAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Linear Equation", result.Name);
            Assert.Equal("Solve for x: 2x + 3 = 7", result.Statement);
        }

        // HIGH PRIORITY TESTS - Input Validation
        [Fact]
        public async Task EvaluateAnswerAsync_WithNonMathematicalAnswer_ReturnsNonMathematicalFeedback()
        {
            // Arrange
            var request = new EvaluateMathAnswerRequestDto
            {
                ProblemId = 1,
                UserAnswer = "I don't know" // Non-mathematical response (now correctly handled)
            };

            var problem = new MathProblem
            {
                Id = 1,
                Solution = "x = 5",
                Explanation = "Test explanation"
            };

            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(1))
                .ReturnsAsync(problem);

            // Act
            var result = await _service.EvaluateAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsCorrect);
            Assert.Contains("Please provide a mathematical answer", result.Feedback);
        }

        [Fact]
        public async Task EvaluateAnswerAsync_WithEmptyAnswer_ReturnsInvalidExpressionFeedback()
        {
            // Arrange
            var request = new EvaluateMathAnswerRequestDto
            {
                ProblemId = 1,
                UserAnswer = "" // Empty answer
            };

            var problem = new MathProblem
            {
                Id = 1,
                Solution = "x = 5",
                Explanation = "Test explanation"
            };

            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(1))
                .ReturnsAsync(problem);

            _mockMathKernelService.Setup(service => service.ValidateMathExpressionAsync(It.IsAny<string>()))
                .ReturnsAsync(false); // Invalid math expression

            // Act
            var result = await _service.EvaluateAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsCorrect);
            Assert.Contains("Your answer is not a valid mathematical expression", result.Feedback);
        }

        [Fact]
        public async Task EvaluateAnswerAsync_WithActualNonAnswerResponse_ReturnsNonMathematicalFeedback()
        {
            // Arrange - Use a response that will match NonAnswerResponses (now fixed to check before sanitization)
            var request = new EvaluateMathAnswerRequestDto
            {
                ProblemId = 1,
                UserAnswer = "I don't know" // This will now match correctly after the bug fix
            };

            var problem = new MathProblem
            {
                Id = 1,
                Solution = "x = 5",
                Explanation = "Test explanation"
            };

            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(1))
                .ReturnsAsync(problem);

            // Act
            var result = await _service.EvaluateAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsCorrect);
            Assert.Contains("Please provide a mathematical answer", result.Feedback);
        }

        [Fact]
        public async Task EvaluateAnswerAsync_WithInvalidMathExpression_ReturnsInvalidExpressionFeedback()
        {
            // Arrange - Use something that's not a non-answer but also not valid math
            var request = new EvaluateMathAnswerRequestDto
            {
                ProblemId = 1,
                UserAnswer = "this is gibberish" // Not in NonAnswerResponses, but invalid math
            };

            var problem = new MathProblem
            {
                Id = 1,
                Solution = "x = 5",
                Explanation = "Test explanation"
            };

            _mockProblemRepo.Setup(repo => repo.GetProblemByIdAsync(1))
                .ReturnsAsync(problem);

            _mockMathKernelService.Setup(service => service.ValidateMathExpressionAsync(It.IsAny<string>()))
                .ReturnsAsync(false); // Invalid math expression

            // Act
            var result = await _service.EvaluateAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsCorrect);
            Assert.Contains("Your answer is not a valid mathematical expression", result.Feedback);
        }
    }
}