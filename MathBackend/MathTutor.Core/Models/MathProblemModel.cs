using MathTutor.Core.Enums;

namespace MathTutor.Core.Models;

public class MathProblemModel
{
    public int Id { get; set; }
    public string Statement { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public int TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
} 