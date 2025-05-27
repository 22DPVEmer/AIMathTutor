using AutoMapper;
using MathTutor.Application.Interfaces;
using MathTutor.Application.Services;
using MathTutor.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MathTutor.Tests
{
    public class TestFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public TestFixture()
        {
            var services = new ServiceCollection();

            // Register mocks for external dependencies
            services.AddSingleton(CreateMockMapper());
            services.AddSingleton(CreateMockMathProblemRepository());
            services.AddSingleton(CreateMockMathTopicRepository());
            services.AddSingleton(CreateMockMathProblemAttemptRepository());
            services.AddSingleton(CreateMockAIService());
            services.AddSingleton(CreateMockKernelProvider());
            services.AddSingleton(CreateMockJsonService());
            services.AddSingleton(CreateMockMathKernelService());

            // Register actual services for testing
            services.AddScoped<IMathProblemService, MathProblemService>();
            services.AddScoped<IProblemGenerationService, ProblemGenerationService>();
            services.AddScoped<IAnswerEvaluationService, AnswerEvaluationService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private IMapper CreateMockMapper()
        {
            var mockMapper = new Mock<IMapper>();
            // Setup mapper behavior as needed for tests
            return mockMapper.Object;
        }

        private IMathProblemRepository CreateMockMathProblemRepository()
        {
            var mockRepo = new Mock<IMathProblemRepository>();
            // Setup repository behavior as needed for tests
            mockRepo.Setup(x => x.GetProblemByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((MathProblem?)null); // Return null to simulate no existing problem
            mockRepo.Setup(x => x.GetProblemsByTopicAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<MathProblem>()); // Return empty list (no existing problems)
            return mockRepo.Object;
        }

        private IMathTopicRepository CreateMockMathTopicRepository()
        {
            var mockRepo = new Mock<IMathTopicRepository>();
            // Setup repository behavior as needed for tests
            mockRepo.Setup(x => x.GetTopicByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new MathTopic { Id = 1, Name = "Test Topic" }); // Return a valid topic
            return mockRepo.Object;
        }

        private IMathProblemAttemptRepository CreateMockMathProblemAttemptRepository()
        {
            var mockRepo = new Mock<IMathProblemAttemptRepository>();
            // Setup repository behavior as needed for tests
            mockRepo.Setup(x => x.GetAttemptsByUserIdAndProblemIdAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new List<MathProblemAttempt>()); // Return empty list (no existing attempts)
            mockRepo.Setup(x => x.GetAttemptsByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<MathProblemAttempt>()); // Return empty list (no existing attempts)
            mockRepo.Setup(x => x.CreateAttemptAsync(It.IsAny<MathProblemAttempt>()))
                .ReturnsAsync(new MathProblemAttempt { Id = 1 }); // Return successful creation
            mockRepo.Setup(x => x.CreateAttemptWithoutProblemAsync(It.IsAny<MathProblemAttempt>(), It.IsAny<string>()))
                .ReturnsAsync(true); // Return successful creation
            mockRepo.Setup(x => x.DeleteAttemptAsync(It.IsAny<int>()))
                .ReturnsAsync(true); // Return successful deletion
            return mockRepo.Object;
        }

        private IAIservice CreateMockAIService()
        {
            var mockService = new Mock<IAIservice>();
            // Setup AI service behavior as needed for tests
            mockService.Setup(x => x.GenerateMathProblemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"statement\":\"Test problem\",\"solution\":\"Test solution\",\"explanation\":\"Test explanation\"}");
            return mockService.Object;
        }

        private IKernelProvider CreateMockKernelProvider()
        {
            var mockProvider = new Mock<IKernelProvider>();
            // Setup kernel provider behavior as needed for tests
            return mockProvider.Object;
        }

        private IJsonService CreateMockJsonService()
        {
            var mockService = new Mock<IJsonService>();
            // Setup JSON service behavior as needed for tests
            return mockService.Object;
        }

        private IMathKernelService CreateMockMathKernelService()
        {
            var mockKernelService = new Mock<IMathKernelService>();
            mockKernelService.Setup(x => x.GenerateMathProblemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"statement\":\"Test problem\",\"solution\":\"Test solution\",\"explanation\":\"Test explanation\"}");
            mockKernelService.Setup(x => x.ValidateMathExpressionAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            mockKernelService.Setup(x => x.CheckExpressionEquivalenceAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockKernelService.Setup(x => x.SimplifyExpressionAsync(It.IsAny<string>()))
                .ReturnsAsync("simplified");
            mockKernelService.Setup(x => x.EvaluateExpressionAsync(It.IsAny<string>()))
                .ReturnsAsync(42.0);
            return mockKernelService.Object;
        }

        public void Dispose()
        {
            // Cleanup resources if needed
        }
    }
}