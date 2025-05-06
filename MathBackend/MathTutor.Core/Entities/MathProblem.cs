using MathTutor.Core.Enums;

namespace MathTutor.Core.Entities;

public class MathProblem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public int TopicId { get; set; }
    public int PointValue { get; set; } = 1; // Points awarded for solving this problem
    public string? AuthorId { get; set; } // ID of the teacher who created the problem

    // Navigation properties
    public virtual MathTopic Topic { get; set; } = null!;
    public virtual ApplicationUser? Author { get; set; } // Teacher who created the problem
    public virtual ICollection<MathProblemAttempt> Attempts { get; set; } = new List<MathProblemAttempt>();
}