using MathTutor.Core.Enums;
using System;

namespace MathTutor.Core.Entities;

public class UserMathProblem
{
    public int Id { get; set; }
    public string Statement { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsCorrect { get; set; }
    public string UserAnswer { get; set; } = string.Empty;

    // Optional relationship with a curated topic
    public int? TopicId { get; set; }

    // Points awarded for solving this problem
    public int PointValue { get; set; } = 1;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual MathTopic? Topic { get; set; }
}