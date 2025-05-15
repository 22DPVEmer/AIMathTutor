using Microsoft.AspNetCore.Identity;

namespace MathTutor.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsVerified { get; set; } = false;
    
    // Navigation properties for math-related entities
    public virtual ICollection<StudentProgress> Progress { get; set; } = new List<StudentProgress>();
    public virtual ICollection<MathProblemAttempt> ProblemAttempts { get; set; } = new List<MathProblemAttempt>();
    public virtual ICollection<UserMathProblem> UserMathProblems { get; set; } = new List<UserMathProblem>();
} 