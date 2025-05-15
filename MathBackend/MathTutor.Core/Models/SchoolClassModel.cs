using System.Collections.Generic;

namespace MathTutor.Core.Models;

// SchoolClassModel for current implementation
public class SchoolClassModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties represented as IDs or simplified DTOs
    public ICollection<MathTopicModel> Topics { get; set; } = new List<MathTopicModel>();
    public int TopicCount { get; set; }
}
