using MathTutor.Core.Enums;

namespace MathTutor.Core.Models;

public class MathTopicModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProblemCount { get; set; }
} 