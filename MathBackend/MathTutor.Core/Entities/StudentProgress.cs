using MathTutor.Core.Enums;

namespace MathTutor.Core.Entities;

public class StudentProgress
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public int PercentageCompleted { get; set; } = 0;
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual MathTopic Topic { get; set; } = null!;
} 