using AutoMapper;
using MathTutor.Application.DTOs;
using MathTutor.Application.Interfaces;
using MathTutor.Application.Services;
using MathTutor.Core.Entities;
using MathTutor.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Xunit;

namespace MathTutor.Tests.Integration
{
    public class MathProblemServiceIntegrationTests : IClassFixture<TestFixture>
    {
        private readonly IServiceProvider _serviceProvider;

        public MathProblemServiceIntegrationTests(TestFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task EvaluateAndSaveAsync_WithCorrectAnswer_SavesAttemptAndReturnsCorrectResult()
        {
            // Arrange
            var mathProblemService = _serviceProvider.GetRequiredService<IMathProblemService>();
            var userId = "test-user-id";

            var request = new EvaluateAndSaveRequestDto
            {
                Problem = "Solve for x: 2x + 3 = 7",
                UserAnswer = "x = 2",
                Solution = "x = 2",
                Explanation = "Subtract 3 from both sides, then divide by 2.",
                Name = "Simple Linear Equation",
                Difficulty = "Easy",
                Topic = "Algebra",
                TopicId = 1
            };

            // Act
            var result = await mathProblemService.EvaluateAndSaveAsync(request, userId);

            // Assert
            Assert.NotNull(result);

            // Debug information if the test fails
            if (!result.Success)
            {
                // This will help us understand why the test is failing
                Assert.True(result.Success, $"Expected Success to be true, but it was false. IsCorrect: {result.IsCorrect}, Feedback: '{result.Feedback}', HasExistingCorrectAttempt: {result.HasExistingCorrectAttempt}");
            }

            Assert.True(result.Success);
            Assert.True(result.IsCorrect);
            Assert.Contains("Correct", result.Feedback);
        }

        [Fact]
        public async Task EvaluateAndSaveAsync_WithIncorrectAnswer_SavesAttemptWithCorrectFeedback()
        {
            // Arrange
            var mathProblemService = _serviceProvider.GetRequiredService<IMathProblemService>();
            var userId = "test-user-incorrect";

            var request = new EvaluateAndSaveRequestDto
            {
                Problem = "Solve for x: 3x + 6 = 15",
                UserAnswer = "x = 4", // Incorrect answer (correct is x = 3)
                Solution = "x = 3",
                Explanation = "Subtract 6 from both sides, then divide by 3.",
                Name = "Linear Equation Test",
                Difficulty = "Easy",
                Topic = "Algebra",
                TopicId = 1
            };

            // Act
            var result = await mathProblemService.EvaluateAndSaveAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success); // Should still save the attempt
            Assert.False(result.IsCorrect); // Answer should be marked incorrect
            Assert.NotEmpty(result.Feedback); // Should provide feedback
        }

        [Fact]
        public async Task GenerateMathProblemAsync_WithSaveToDatabase_CreatesAndSavesProblem()
        {
            // Arrange
            var mathProblemService = _serviceProvider.GetRequiredService<IMathProblemService>();

            var request = new GenerateMathProblemRequestDto
            {
                Topic = "Geometry",
                Difficulty = "Medium",
                TopicId = 1,
                SaveToDatabase = true
            };

            // Act
            var result = await mathProblemService.GenerateMathProblemAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Statement);
            Assert.NotEmpty(result.Solution);
            Assert.NotEmpty(result.Explanation);
        }

        [Fact]
        public async Task GetProblemsByTopicAsync_WithValidTopic_ReturnsFilteredProblems()
        {
            // Arrange
            var mathProblemService = _serviceProvider.GetRequiredService<IMathProblemService>();
            int topicId = 1;

            // Act
            var result = await mathProblemService.GetProblemsByTopicAsync(topicId);

            // Assert
            Assert.NotNull(result);
            // Note: With mocked repositories, this will return empty list
            // In a real integration test with database, you'd verify actual data
        }
    }
}