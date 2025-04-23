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
    public int? ParentTopicId { get; set; }
    public string ParentTopicName { get; set; } = string.Empty;
    public int GradeLevel { get; set; } = 0;
    public int ProblemCount { get; set; }
    public int TotalPointsPossible { get; set; } = 0;
    public List<MathTopicModel> Subtopics { get; set; } = new List<MathTopicModel>();
} 