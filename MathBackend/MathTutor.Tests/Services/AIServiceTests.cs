using MathTutor.Application.Interfaces;
using MathTutor.Application.Services;
using Moq;
using Xunit;

namespace MathTutor.Tests.Services
{
    public class AIServiceTests
    {
        private readonly Mock<IProblemGenerationService> _mockProblemGenerationService;
        private readonly Mock<IAnswerEvaluationService> _mockAnswerEvaluationService;
        private readonly Mock<IGuidanceService> _mockGuidanceService;
        private readonly Mock<IKernelProvider> _mockKernelProvider;
        private readonly AIservice _service;

        public AIServiceTests()
        {
            _mockProblemGenerationService = new Mock<IProblemGenerationService>();
            _mockAnswerEvaluationService = new Mock<IAnswerEvaluationService>();
            _mockGuidanceService = new Mock<IGuidanceService>();
            _mockKernelProvider = new Mock<IKernelProvider>();

            _service = new AIservice(
                _mockProblemGenerationService.Object,
                _mockAnswerEvaluationService.Object,
                _mockGuidanceService.Object,
                _mockKernelProvider.Object);
        }

        [Fact]
        public async Task GenerateMathProblemAsync_DelegatesCorrectly()
        {
            // Arrange
            string topic = "Calculus";
            string difficulty = "Hard";
            string expectedResponse = "{\"statement\":\"Find the derivative of f(x) = x³ + 2x² - 5x + 3\",\"solution\":\"f'(x) = 3x² + 4x - 5\",\"explanation\":\"Use the power rule\"}";
            
            _mockProblemGenerationService.Setup(service => 
                service.GenerateMathProblemAsync(topic, difficulty))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.GenerateMathProblemAsync(topic, difficulty);

            // Assert
            Assert.Equal(expectedResponse, result);
            _mockProblemGenerationService.Verify(service => 
                service.GenerateMathProblemAsync(topic, difficulty), Times.Once);
        }

        [Fact]
        public async Task EvaluateAnswerAsync_DelegatesCorrectly()
        {
            // Arrange
            string problem = "Solve for x: 2x + 5 = 15";
            string userAnswer = "x = 5";
            string expectedResponse = "{\"isCorrect\":true,\"feedback\":\"Correct! You solved the equation properly.\"}";
            
            _mockAnswerEvaluationService.Setup(service => 
                service.EvaluateAnswerAsync(problem, userAnswer))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.EvaluateAnswerAsync(problem, userAnswer);

            // Assert
            Assert.Equal(expectedResponse, result);
            _mockAnswerEvaluationService.Verify(service => 
                service.EvaluateAnswerAsync(problem, userAnswer), Times.Once);
        }
    }
}