namespace MathTutor.Core.Entities;

public class MathProblemAttempt
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProblemId { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; } = 0; // Points earned for this attempt
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual MathProblem Problem { get; set; } = null!;
} 