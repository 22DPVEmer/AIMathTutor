using MathTutor.Core.Enums;
using System.Text.Json.Serialization;

namespace MathTutor.Application.DTOs
{
    public class MathProblemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Statement { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
    }

    public class CreateMathProblemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Statement { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
        public int TopicId { get; set; }
        public int PointValue { get; set; } = 1;
    }

    public class UpdateMathProblemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Statement { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public int TopicId { get; set; }
    }

    public class GenerateMathProblemRequestDto
    {
        public string Topic { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Medium";
        [JsonIgnore]
        public int TopicId { get; set; }
        public bool SaveToDatabase { get; set; }
    }

    public class GeneratedMathProblemResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Statement { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }

    public class EvaluateMathAnswerRequestDto
    {
        public int ProblemId { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
    }

    public class EvaluateMathAnswerResponseDto
    {
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }

    public class SaveProblemAttemptDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Statement { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int? TopicId { get; set; }
    }

    // DirectEvaluationRequestDto is already defined elsewhere

    public class GuidanceRequestDto
    {
        public string Problem { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }

    public class GuidanceResponseDto
    {
        public string Guidance { get; set; } = string.Empty;
    }
}