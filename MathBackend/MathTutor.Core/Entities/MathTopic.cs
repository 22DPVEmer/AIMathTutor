using MathTutor.Core.Enums;

namespace MathTutor.Core.Entities;

public class MathTopic
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public int CategoryId { get; set; }
    
    // Navigation properties
    public virtual MathCategory Category { get; set; } = null!;
    public virtual ICollection<MathProblem> Problems { get; set; } = new List<MathProblem>();
    public virtual ICollection<StudentProgress> StudentProgress { get; set; } = new List<StudentProgress>();
} 