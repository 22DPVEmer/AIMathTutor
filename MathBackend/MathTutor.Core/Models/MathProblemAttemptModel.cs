namespace MathTutor.Core.Models;

public class MathProblemAttemptModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int ProblemId { get; set; }
    public string ProblemStatement { get; set; } = string.Empty;
    public string UserAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime AttemptedAt { get; set; }
} 