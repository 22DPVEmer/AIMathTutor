using MathTutor.Core.Enums;
using System;

namespace MathTutor.Core.Models
{
    public class UserMathProblemModel
    {
        public int Id { get; set; }
        public string Statement { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsCorrect { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int? TopicId { get; set; }
    }


}
