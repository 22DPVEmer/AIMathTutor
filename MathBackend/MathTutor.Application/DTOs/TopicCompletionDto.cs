using System;

namespace MathTutor.Application.DTOs
{
    public class TopicCompletionDto
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public int TotalPointsPossible { get; set; }
        public int PointsEarned { get; set; }
        public int PercentageCompleted { get; set; }
    }
}

