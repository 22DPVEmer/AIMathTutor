using MathTutor.Application.Interfaces;
using MathTutor.Application.Services;
using Moq;
using Xunit;

namespace MathTutor.Tests.Services
{
    public class ProblemGenerationServiceTests
    {
        private readonly Mock<IKernelProvider> _mockKernelProvider;
        private readonly Mock<IJsonService> _mockJsonService;
        private readonly ProblemGenerationService _service;

        public ProblemGenerationServiceTests()
        {
            _mockKernelProvider = new Mock<IKernelProvider>();
            _mockJsonService = new Mock<IJsonService>();
            
            _service = new ProblemGenerationService(
                _mockKernelProvider.Object,
                _mockJsonService.Object);
        }

        [Fact]
        public async Task GenerateMathProblemAsync_ReturnsValidResponse()
        {
            // Arrange
            string topic = "Algebra";
            string difficulty = "Medium";
            string aiResponse = "{\"statement\":\"Solve for x: 3x + 7 = 22\",\"solution\":\"x = 5\",\"explanation\":\"Subtract 7 from both sides, then divide by 3.\"}";
            
            _mockKernelProvider.Setup(provider => 
                provider.InvokePromptAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<int>()))
                .ReturnsAsync(aiResponse);
                
            _mockJsonService.Setup(service => service.IsValidJson(aiResponse))
                .Returns(true);
                
            _mockJsonService.Setup(service => 
                service.HasRequiredProperties(aiResponse, It.IsAny<string>()))
                .Returns(true);
                
            _mockJsonService.Setup(service => 
                service.HasRequiredProperties(aiResponse, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            // Act
            var result = await _service.GenerateMathProblemAsync(topic, difficulty);

            // Assert
            Assert.Equal(aiResponse, result);
            _mockKernelProvider.Verify(provider => 
                provider.InvokePromptAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<int>()), 
                Times.Once);
        }

        [Fact]
        public async Task GenerateMathProblemAsync_WithInvalidJson_ReturnsFallback()
        {
            // Arrange
            string topic = "Algebra";
            string difficulty = "Medium";
            string aiResponse = "This is not valid JSON";
            
            _mockKernelProvider.Setup(provider => 
                provider.InvokePromptAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<int>()))
                .ReturnsAsync(aiResponse);
                
            _mockJsonService.Setup(service => service.IsValidJson(aiResponse))
                .Returns(false);

            // Act
            var result = await _service.GenerateMathProblemAsync(topic, difficulty);

            // Assert
            Assert.NotEqual(aiResponse, result);
            Assert.Contains("statement", result.ToLower());
            Assert.Contains("solution", result.ToLower());
        }
    }
}