using MathTutor.Core.Enums;

namespace MathTutor.Core.Models;

public class StudentProgressModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int PercentageCompleted { get; set; }
    public ProgressStatus Status { get; set; }
    public DateTime LastUpdated { get; set; }
} 