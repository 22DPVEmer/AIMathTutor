using MathTutor.Core.Enums;

namespace MathTutor.Core.Entities;

public class MathTopic
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public int CategoryId { get; set; }
    public int? ParentTopicId { get; set; }
    public int GradeLevel { get; set; } = 0; // Grade level (7-12)
    public int TotalPointsPossible { get; set; } = 0; // Total points possible for all problems in this topic
    
    // Navigation properties
    public virtual MathCategory Category { get; set; } = null!;
    public virtual MathTopic? ParentTopic { get; set; }
    public virtual ICollection<MathTopic> Subtopics { get; set; } = new List<MathTopic>();
    public virtual ICollection<MathProblem> Problems { get; set; } = new List<MathProblem>();
    public virtual ICollection<StudentProgress> StudentProgress { get; set; } = new List<StudentProgress>();
} 