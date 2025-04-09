using MathTutor.Core.Enums;

namespace MathTutor.Core.Entities;

public class MathProblem
{
    public int Id { get; set; }
    public string Statement { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public int TopicId { get; set; }
    
    // Navigation properties
    public virtual MathTopic Topic { get; set; } = null!;
    public virtual ICollection<MathProblemAttempt> Attempts { get; set; } = new List<MathProblemAttempt>();
} 